
using Godot;

public partial class VisualTileMapLayer : TileMapLayer
{
    [Export]
    private TileMapLayer tileMapLayerToEcho;

    private MapController mapController;

    public override void _Ready()
    {
        mapController = InjectionManager.Get<MapController>();
        tileMapLayerToEcho.Changed += Echo;
    }

    private void Echo()
    {
        foreach (var visibleCell in mapController.GetVisibleCells())
        {
            var sourceId = tileMapLayerToEcho.GetCellSourceId(visibleCell);
            var atlasCoords = tileMapLayerToEcho.GetCellAtlasCoords(visibleCell);
            var altTile = tileMapLayerToEcho.GetCellAlternativeTile(visibleCell);
            
            if (sourceId != this.GetCellSourceId(visibleCell) || atlasCoords != this.GetCellAtlasCoords(visibleCell) || altTile != this.GetCellAlternativeTile(visibleCell))
                this.SetCell(visibleCell, sourceId, atlasCoords, altTile);
        }
    }
}
