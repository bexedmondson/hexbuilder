using System.Collections.Generic;
using Godot;

public partial class InventoryManager : Node2D, IInjectable
{
    [Export]
    private CurrencyDisplay inventoryDisplay;
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, int> startAmounts;

    private CurrencySum inventory = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);
    }
    
    public void OnNewGame()
    {
        inventory.Clear();
        inventory = new CurrencySum(startAmounts);
        
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }

    public bool CanAfford(CurrencySum price)
    {
        foreach (var kvp in price)
        {
            if (kvp.Value > inventory[kvp.Key])
                return false;
        }

        return true;
    }
    
    public void SpendCurrency(CurrencySum price)
    {
        foreach (var kvp in price)
        {
            inventory[kvp.Key] -= kvp.Value;
        }
        
        inventoryDisplay.Cleanup(); //obviously temporary
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }

    //TODO want to combine this with above method but i couldn't think of a good name for the method lol
    public void OnNextTurn(CurrencySum currencyDelta)
    {
        inventory.Add(currencyDelta);
        
        inventoryDisplay.Cleanup(); //obviously temporary
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }
}