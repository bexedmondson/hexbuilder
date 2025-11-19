using System.Collections.Generic;
using Godot;

public class MapCurrencyChangeAnalyser : IInjectable
{
    private MapController mapController;
    public MapCurrencyChangeAnalyser(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    //TODO: cache this and full turn change to minimise recalculation?
    public List<CurrencySum> GetFullCurrencyTurnDeltas()
    {
        List<CurrencySum> allCurrencyChanges = new();
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
            if (cellTileData?.baseTurnCurrencyChange != null)
                allCurrencyChanges.Add(new CurrencySum(cellTileData.baseTurnCurrencyChange));
            
            allCurrencyChanges.Add(CalculateAdjacencyEffects(cell, cellTileData));
        }
        
        return allCurrencyChanges;
    }

    public CurrencySum GetOverallTurnDelta()
    {
        CurrencySum turnChange = new();
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
            if (cellTileData?.baseTurnCurrencyChange != null)
                turnChange.Add(new CurrencySum(cellTileData.baseTurnCurrencyChange));
            
            turnChange.Add(CalculateAdjacencyEffects(cell, cellTileData));
        }

        return turnChange;
    }
    
    private CurrencySum CalculateAdjacencyEffects(Vector2I centreCell, CustomTileData centreTileData)
    {
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
                
                if (relevantAdjacenciesByAdjacentData.TryGetValue(neighbourData, out var relevantAdjacency))
                {
                    adjacencyEffects.Add(relevantAdjacency.currencyEffect);
                }
            }
        }
        
        return adjacencyEffects;
    }
}
