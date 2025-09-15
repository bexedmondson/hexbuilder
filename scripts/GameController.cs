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
        CurrencySum totalCurrencyChange = new CurrencySum();
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
            totalCurrencyChange.Add(cellTileData.baseTurnCurrencyChange);
        }
        
        inventoryManager.OnNextTurn(totalCurrencyChange);
    }
}