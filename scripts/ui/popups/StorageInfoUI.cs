using Godot;

public partial class StorageInfoUI : Control
{
    [Export]
    private CurrencyDisplay storageCurrencyDisplay;
    
    public void SetStorageInfo(StorageCapacityDataComponent storageCapacityComponent)
    {
        if (storageCapacityComponent.capacities == null || storageCapacityComponent.capacities.Count == 0)
            return;
        
        storageCurrencyDisplay.DisplayCurrencyAmount(new CurrencySum(storageCapacityComponent.capacities));
    }
}
