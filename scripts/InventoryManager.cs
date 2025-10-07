using System.Collections.Generic;
using Godot;

public partial class InventoryManager : Node2D, IInjectable
{
    [Export]
    private CurrencyDisplay inventoryDisplay;
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, int> startAmounts;

    private CurrencySum inventory = new();

    private InventoryStatsTracker statsTracker;

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);

        statsTracker = new InventoryStatsTracker();
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        InjectionManager.Deregister(this);
        statsTracker = null;
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
        inventory.Subtract(price);
        
        inventoryDisplay.Cleanup(); //obviously temporary, this is a horrendously inefficient way to handle this
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }

    public void OnNextTurn(CurrencySum[] allCurrencyChanges)
    {
        foreach (var currencyChange in allCurrencyChanges)
        {
            inventory.Add(currencyChange);
            statsTracker.OnCurrencyChange(currencyChange);
        }

        inventoryDisplay.Cleanup(); //obviously temporary
        inventoryDisplay.DisplayCurrencyAmount(inventory);
    }
}