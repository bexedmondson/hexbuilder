using System.Collections.Generic;
using Godot;

public partial class GameController : Node2D
{
    [Export]
    private MapController mapController;

    [Export]
    private InventoryManager inventoryManager;

    private TileDatabase tileDatabase;

    public override void _EnterTree()
    {
        base._EnterTree();

        tileDatabase = InjectionManager.Get<TileDatabase>();
        tileDatabase ??= new TileDatabase();
    }

    public override void _Ready()
    {
        base._Ready();
        
        mapController.OnNewGame();
        inventoryManager.OnNewGame();
    }

    public void NextTurn()
    {
        List<CurrencySum> allCurrencyChanges = new();
        
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
            if (cellTileData?.baseTurnCurrencyChange != null)
                allCurrencyChanges.Add(new CurrencySum(cellTileData.baseTurnCurrencyChange));
            
            allCurrencyChanges.Add(mapController.CalculateAdjacencyEffects(cell, cellTileData));
        }
        
        inventoryManager.OnNextTurn(allCurrencyChanges.ToArray());
    }
}