using Godot;

public partial class LockedCellPopup : Popup
{
    [Export]
    private CurrencyDisplay priceDisplay;

    [Export]
    private Button confirmButton;

    private MapController mapController;
    private InventoryManager inventoryManager;
    
    private Vector2I cell;

    public override void _Ready()
    {
        this.SetVisible(false);
        mapController = InjectionManager.Get<MapController>();
        inventoryManager = InjectionManager.Get<InventoryManager>();
    }

    public void ShowForCell(Vector2I setCell)
    {
        cell = setCell;
        var cellUnlockCost = mapController.GetCellUnlockCost(cell);
        
        priceDisplay.DisplayCurrencyAmount(cellUnlockCost);

        bool canAfford = inventoryManager.CanAfford(cellUnlockCost);
        confirmButton.Disabled = !canAfford;
        
        this.SetVisible(true);
    }

    public override void Confirm()
    {
        var cellUnlockCost = mapController.GetCellUnlockCost(cell);
        if (!inventoryManager.CanAfford(cellUnlockCost))
            return;

        mapController.UnlockCell(cell);
        inventoryManager.SpendCurrency(cellUnlockCost);
        Close();
    }

    public override void Close()
    {
        base.Close();
        this.SetVisible(false);
        priceDisplay.Cleanup();
        InjectionManager.Get<MapHighlightController>().Clear();
    }
}