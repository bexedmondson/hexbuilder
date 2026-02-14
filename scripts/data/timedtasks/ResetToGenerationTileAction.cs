using Godot;

[GlobalClass]
public partial class ResetToGenerationTileAction : ChangeTileTypeTileAction
{
    public override CustomTileData GetTileToChangeTo(Vector2I cell)
    {
        var mapController = InjectionManager.Get<MapController>();
        return mapController.GetDefaultGeneratedTileAtCell(cell);
    }
}