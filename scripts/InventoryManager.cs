using System.Collections.Generic;
using Godot;

public partial class InventoryManager : Node2D, IInjectable
{
    [Export]
    private CurrencyDisplay inventoryDisplay;
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, int> startAmounts;

    private Dictionary<CurrencyType, int> inventory = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);
    }
    
    public void OnNewGame()
    {
        inventory.Clear();
        inventory = new Dictionary<CurrencyType, int>(startAmounts);
        
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }

    public bool CanAfford(Dictionary<CurrencyType, int> price)
    {
        foreach (var kvp in price)
        {
            if (kvp.Value > inventory[kvp.Key])
                return false;
        }

        return true;
    }
    
    public void SpendCurrency(Dictionary<CurrencyType, int> price)
    {
        foreach (var kvp in price)
        {
            inventory[kvp.Key] -= kvp.Value;
        }
        
        inventoryDisplay.Cleanup(); //obviously temporary
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }
}