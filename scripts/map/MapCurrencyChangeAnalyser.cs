using System.Collections.Generic;
using Godot;

public class MapCurrencyChangeAnalyser : IInjectable
{
    private MapController mapController;
    private WorkplaceManager workplaceManager;
    private HousingManager housingManager;
    
    public MapCurrencyChangeAnalyser(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
        
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        housingManager ??= InjectionManager.Get<HousingManager>();
    }

    public List<CurrencySum> GetFullCurrencyTurnDeltas()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        housingManager ??= InjectionManager.Get<HousingManager>();
        
        List<CurrencySum> allCurrencyChanges = new();
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            List<CurrencySum> cellCurrencyChanges = GetCellCurrencyChanges(cell);

            if (cellCurrencyChanges.IsNullOrEmpty())
                continue;
            
            allCurrencyChanges.AddRange(cellCurrencyChanges);
        }
        
        return allCurrencyChanges;
    }

    public CurrencySum GetTotalCellCurrencyChange(Vector2I cell)
    {
        CurrencySum sum = new();
        var changes = GetCellCurrencyChanges(cell);
        foreach (var change in changes)
        {
            sum += change;
        }
        return sum;
    }

    private List<CurrencySum> GetCellCurrencyChanges(Vector2I cell)
    {
        if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
            return null;

        var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
        if (cellTileData == null)
            return null;

        List<CurrencySum> cellCurrencyChanges = new();

        CurrencySum baseCurrencyChange = GetActualCellBaseCurrencyChange(cell, cellTileData);
        if (baseCurrencyChange != null && baseCurrencyChange.Count > 0)
            cellCurrencyChanges.Add(baseCurrencyChange);
            
        //all the below are production bonuses, so with no production we can early exit here
        if (IsCellProductionProhibited(cell, cellTileData))
            return cellCurrencyChanges;

        if (TryGetWorkplaceMaxWorkerBonus(cell, cellTileData, out CurrencySum maxWorkerBonus))
        {
            cellCurrencyChanges.Add(maxWorkerBonus);
        }
            
        var adjacencyEffects = CalculateAdjacencyEffects(cell, cellTileData);
        if (adjacencyEffects.Count > 0)
            cellCurrencyChanges.Add(adjacencyEffects);

        return cellCurrencyChanges;
    }

    public CurrencySum GetActualCellBaseCurrencyChange(Vector2I cell, CustomTileData cellTileData)
    {
        if (cellTileData.baseTurnCurrencyChange.IsNullOrEmpty())
            return null;
        
        bool noProduction = IsCellProductionProhibited(cell, cellTileData);
        bool noConsumption = IsCellConsumptionProhibited(cell, cellTileData);

        if (!noProduction && !noConsumption)
            return new CurrencySum(cellTileData.baseTurnCurrencyChange);

        CurrencySum finalBaseDelta = new();
        foreach (var kvp in cellTileData.baseTurnCurrencyChange)
        {
            if (kvp.Value > 0 && !noProduction)
                finalBaseDelta.Add(kvp.Key, kvp.Value);
            else if (kvp.Value < 0 && !noConsumption)
                finalBaseDelta.Add(kvp.Key, kvp.Value);
        }

        return finalBaseDelta;
    }

    public bool IsCellProductionProhibited(Vector2I cell, CustomTileData cellTileData)
    {
        //if it's not a workplace, can't be affected by lack of workers
        if (!cellTileData.TryGetComponent(out WorkerCapacityComponent _))
            return false;
        
        //don't know how we'd get here without the cell being a workplace but whatever
        bool isWorkplace = workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplace);
        
        //if there aren't any workers at a workplace then can't produce anything
        //TODO rework workerCount's effects on currency change maybe?
        return !isWorkplace || workplace.workerCount <= 0;
    }

    public bool IsCellConsumptionProhibited(Vector2I cell, CustomTileData cellTileData)
    {
        //don't apply consumption effects for residences without residents in them
        if (housingManager.TryGetHouseOnCell(cell, out var house))
            return house.occupants.Length <= 0;

        //don't apply consumption effects for workplaces without workers in them
        //update: above is true, but no production warning from no workers is more important. removing for now 
        //if (workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplace))
        //    return workplace.workerCount <= 0;

        return false;
    }

    private bool TryGetWorkplaceMaxWorkerBonus(Vector2I cell, CustomTileData cellTileData, out CurrencySum result)
    {
        if (workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplace)
            && workplace.workerCount == workplace.capacity
            && cellTileData.TryGetComponent(out MaximumWorkerProductionBonusComponent maxWorkerBonus))
        {
            //no max worker bonus if any of the workers are maximum depressed
            foreach (var worker in workplace.workers)
            {
                if (worker.happiness <= ResidentState.minHappiness)
                {
                    result = null;
                    return false;
                }
            }
            
            result = new CurrencySum(maxWorkerBonus.extraBaseProduction);
            return true;
        }

        result = null;
        return false;
    }
    
    private CurrencySum CalculateAdjacencyEffects(Vector2I centreCell, CustomTileData centreTileData)
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        
        CurrencySum adjacencyEffects = new();

        Dictionary<int, Dictionary<CustomTileData, AdjacencyConfig>> adjacenciesByDistance = new();

        if (centreTileData?.adjacencies == null)
            return adjacencyEffects;
        
        foreach (var adjacency in centreTileData.adjacencies)
        {
            if (adjacenciesByDistance.ContainsKey(adjacency.distance))
                adjacenciesByDistance[adjacency.distance].Add(adjacency.requiredTile, adjacency);
            else
                adjacenciesByDistance[adjacency.distance] = new(){ {adjacency.requiredTile, adjacency} };
        }

        foreach (var kvp in adjacenciesByDistance)
        {
            var distance = kvp.Key;
            var relevantAdjacenciesByAdjacentData = kvp.Value;
            
            var neighboursByDistance = mapController.GetSurroundingCellsUpToDistance(centreCell, distance);

            foreach (var neighbour in neighboursByDistance.Keys)
            {
                var neighbourData = mapController.BaseMapLayer.GetCellCustomData(neighbour);
                if (neighbourData == null) 
                    continue;
                
                if (mapController.GetCellStatus(neighbour) != CellStatus.Unlocked)
                    continue;

                if (!relevantAdjacenciesByAdjacentData.TryGetValue(neighbourData, out var relevantAdjacency))
                    continue;

                //workplaces' effects are dependent on if there are workers there - otherwise, the effects just happen
                if (neighbourData.IsWorkplace)
                {
                    if (!workplaceManager.TryGetWorkplaceAtLocation(neighbour, out var neighbourWorkplace))
                    {
                        GD.PushError("workplace data from tile data but no workplace found here?? something's gone horribly wrong");
                        continue;
                    }
                    
                    adjacencyEffects.Add(neighbourWorkplace.GetWorkerDependentAdjacencyEffects(centreTileData));
                }
                else
                {
                    adjacencyEffects.Add(relevantAdjacency.currencyEffect);
                }
            }
        }
        
        return adjacencyEffects;
    }

    public CurrencySum GetOverallTurnDelta()
    {
        var fullCurrencyTurnDeltas = GetFullCurrencyTurnDeltas();
        
        CurrencySum turnChange = new();
        foreach (var turnDelta in fullCurrencyTurnDeltas)
        {
            turnChange.Add(turnDelta);
        }
        
        return turnChange;
    }
}
