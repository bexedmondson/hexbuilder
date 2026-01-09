using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class StorageRequirement : Requirement
{
    [Export]
    private Dictionary<CurrencyType, int> storageRequired;

    public override bool IsSatisfied()
    {
        var inventoryManager = InjectionManager.Get<InventoryManager>();
        foreach (var kvp in storageRequired)
        {
            if (kvp.Value < inventoryManager.GetCurrencyCapacity(kvp.Key))
                return false;
        }
        
        return true;
    }
}