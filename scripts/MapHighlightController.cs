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
    [Export]
    private int adjacencyDrawbackSceneIndex;
    
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
        SetCell(cell, TileSet.GetSourceId(adjacencySourceId), Vector2I.Zero, adjacencyBenefitSceneIndex);
    }
    
    public void OnHighlightDrawbackTile(Vector2I cell)
    {
        SetCell(cell, TileSet.GetSourceId(adjacencySourceId), Vector2I.Zero, adjacencyDrawbackSceneIndex);
    }

    public void ClearAllExcept(Vector2I cell)
    {
        foreach (var usedCell in GetUsedCells())
        {
            if (usedCell != cell)
                EraseCell(cell);
        }
    }
}
