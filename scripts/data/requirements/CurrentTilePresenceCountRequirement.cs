using System.Collections.Generic;
using Godot;

[GlobalClass][Tool]
public partial class CurrentTilePresenceCountRequirement : Requirement
{
    [Export]
    public TileRequirementAmount requiredTileBuildCount = new();
    
    public override bool IsSatisfied()
    {
        var baseTileMapLayer = InjectionManager.Get<MapController>().BaseMapLayer;
        int currentTileCount = 0;
        
        foreach (var cell in baseTileMapLayer.GetUsedCells())
        {
            var tileData = baseTileMapLayer.GetCellCustomData(cell);
            if (tileData == null || requiredTileBuildCount.tile == tileData)
                continue;
            
            currentTileCount++;
        }
        
        return requiredTileBuildCount.IsPass(currentTileCount);
    }
}

