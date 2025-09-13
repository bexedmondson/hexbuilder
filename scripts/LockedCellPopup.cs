using Godot;

public partial class LockedCellPopup : Control
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

    public void SetCell(Vector2I setCell)
    {
        cell = setCell;
        var cellUnlockCost = mapController.GetCellUnlockCost(cell);
        
        priceDisplay.DisplayCurrencyAmount(cellUnlockCost);

        bool canAfford = inventoryManager.CanAfford(cellUnlockCost);
        confirmButton.Disabled = !canAfford;
    }

    public void ConfirmUnlock()
    {
        var cellUnlockCost = mapController.GetCellUnlockCost(cell);
        if (!inventoryManager.CanAfford(cellUnlockCost))
            return;

        mapController.UnlockCell(cell);
        Close();
        //inventoryManager.SpendCurrency(cellUnlockCost);
    }

    public void Close()
    {
        this.SetVisible(false);
        priceDisplay.Cleanup();
    }
}