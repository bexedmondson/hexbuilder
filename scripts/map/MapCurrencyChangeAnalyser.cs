using System.Collections.Generic;
using Godot;

public class MapCurrencyChangeAnalyser : IInjectable
{
    private MapController mapController;
    private WorkplaceManager workplaceManager;
    
    public MapCurrencyChangeAnalyser(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public List<CurrencySum> GetFullCurrencyTurnDeltas()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        
        List<CurrencySum> allCurrencyChanges = new();
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);

            //TODO rework workerCount's effects on currency change?
            if (workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplace) && workplace.workerCount <= 0)
            {
                continue;
            }
            //don't get regular effects nor adjacency benefits for unstaffed workplaces
            //non-workplace tiles that have effects are fine though
            
            if (cellTileData?.baseTurnCurrencyChange != null)
                allCurrencyChanges.Add(new CurrencySum(cellTileData.baseTurnCurrencyChange));
            
            allCurrencyChanges.Add(CalculateAdjacencyEffects(cell, cellTileData));
        }
        
        return allCurrencyChanges;
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
            {
                adjacenciesByDistance[adjacency.distance].Add(adjacency.requiredTile, adjacency);
            }
            else
            {
                adjacenciesByDistance[adjacency.distance] = new(){
                    { adjacency.requiredTile, adjacency }
                };
            }
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
}
