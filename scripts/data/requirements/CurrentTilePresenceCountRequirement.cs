using System.Collections.Generic;
using Godot;

[GlobalClass][Tool]
public partial class CurrentTilePresenceCountRequirement : Requirement
{
    [Export]
    public Godot.Collections.Array<TileRequirementAmount> requiredTileBuildCounts = new();

    Dictionary<CustomTileData, TileRequirementAmount> relevantTiles = new();
    
    public override bool IsSatisfied()
    {
        var baseTileMapLayer = InjectionManager.Get<MapController>().BaseMapLayer;

        if (relevantTiles.Count == 0) //building access map
        {
            foreach (var tileRequirementAmount in requiredTileBuildCounts)
            {
                relevantTiles[tileRequirementAmount.tile] = tileRequirementAmount;
            }
        }

        System.Collections.Generic.Dictionary<CustomTileData, int> currentTileCount = new();
        
        foreach (var cell in baseTileMapLayer.GetUsedCells())
        {
            var tileData = baseTileMapLayer.GetCellCustomData(cell);
            if (tileData == null || !relevantTiles.ContainsKey(tileData))
                continue;
            
            if (!currentTileCount.TryAdd(tileData, 1))
                currentTileCount[tileData]++;
        }

        foreach (var kvp in relevantTiles)
        {
            if (!currentTileCount.TryGetValue(kvp.Key, out var currentValue) || !currentValue.IsPass(kvp.Value.comparison, kvp.Value.amount))
                return false;
        }

        return true;
    }
}

