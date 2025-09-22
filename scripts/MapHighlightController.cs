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
    private int adjacencyBenefitSceneIndex;
    
    private Vector2I adjacencyTileCoords = Vector2I.Zero; //SetCell docs specify this for scene tiles
    
    public override void _EnterTree()
    {
        InjectionManager.Register(this);
    }

    public void OnSelectTile(Vector2I cell)
    {
        Clear();
        SetCell(cell, TileSet.GetSourceId(selectionSourceId), selectionTileCoords);
    }

    public void OnHighlightBenefitTile(Vector2I cell)
    {
        SetCell(cell, TileSet.GetSourceId(selectionSourceId), selectionTileCoords);
    }
}
