using Godot;

public partial class GameController : Node2D
{
    [Export]
    private MapController mapController;

    [Export]
    private InventoryManager inventoryManager;

    private TileDatabase tileDatabase = new TileDatabase();

    public override void _Ready()
    {
        base._Ready();
        
        mapController.OnNewGame();
        inventoryManager.OnNewGame();
    }

    public void NextTurn()
    {
        
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
        }
    }
}