using System.Collections.Generic;
using Godot;

[GlobalClass][Tool]
public partial class CurrentTilePresenceCountUnlockRequirement : UnlockRequirement
{
    [Export]
    public Godot.Collections.Dictionary<CustomTileData, int> requiredTilesBuildCounts = new();

    public override bool IsSatisfied()
    {
        var baseTileMapLayer = InjectionManager.Get<MapController>().BaseMapLayer;

        System.Collections.Generic.Dictionary<CustomTileData, int> currentTileCount = new();
        
        foreach (var cell in baseTileMapLayer.GetUsedCells())
        {
            var tileData = baseTileMapLayer.GetCellCustomData(cell);
            if (tileData == null || !requiredTilesBuildCounts.ContainsKey(tileData))
                continue;
            
            if (!currentTileCount.TryAdd(tileData, 1))
                currentTileCount[tileData]++;
        }

        foreach (var kvp in requiredTilesBuildCounts)
        {
            if (!currentTileCount.TryGetValue(kvp.Key, out var currentValue) || kvp.Value > currentValue)
                return false;
        }

        return true;
    }
}

