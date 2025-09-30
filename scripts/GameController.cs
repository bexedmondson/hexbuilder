using System.Collections.Generic;
using Godot;

public partial class GameController : Node2D
{
    [Export]
    private MapController mapController;

    [Export]
    private InventoryManager inventoryManager;

    public override void _Ready()
    {
        base._Ready();
        
        mapController.OnNewGame();
        inventoryManager.OnNewGame();
    }

    public void NextTurn()
    {
        CurrencySum totalCurrencyChange = new CurrencySum();
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
            totalCurrencyChange.Add(cellTileData.baseTurnCurrencyChange);
            
            totalCurrencyChange.Add(CalculateAdjacencyEffects(cell, cellTileData));
        }
        
        inventoryManager.OnNextTurn(totalCurrencyChange);
    }

    private CurrencySum CalculateAdjacencyEffects(Vector2I centreCell, CustomTileData centreTileData)
    {
        CurrencySum adjacencyEffects = new();

        Dictionary<int, Dictionary<CustomTileData, AdjacencyConfig>> adjacenciesByDistance = new();

        if (centreTileData.adjacencies == null)
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