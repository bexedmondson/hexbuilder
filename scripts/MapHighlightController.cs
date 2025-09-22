using Godot;

public partial class MapHighlightController : TileMapLayer, IInjectable
{
    [ExportGroup("Selection Highlight")]
    [Export]
    private int selectionSourceId;
    [Export]
    private Vector2I selectionTileCoords;
    
    [ExportGroup("Adjacency Highlight")]
    [Export]
    private int adjacencySourceId;
    [Export]
    private Vector2I adjacencyTileCoords;
    
    public override void _EnterTree()
    {
        InjectionManager.Register(this);
    }

    public void OnSelectTile(Vector2I cell)
    {
        SetCell(cell, TileSet.GetSourceId(selectionSourceId), selectionTileCoords);
    }
}
