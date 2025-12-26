using System.Collections.Generic;
using Godot;

public partial class InventoryManager : Node2D, IInjectable
{
    [Export]
    private MainInventoryDisplay mainInventoryDisplay;
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Texture2D> currencyIcons;
    
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
        
        mainInventoryDisplay.FullUpdate(inventory);
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
        
        mainInventoryDisplay.FullUpdate(inventory);
    }

    public Texture2D GetIcon(CurrencyType currencyType)
    {
        return currencyIcons[currencyType];
    }

    public void OnNextTurn(CurrencySum[] allCurrencyChanges)
    {
        foreach (var currencyChange in allCurrencyChanges)
        {
            inventory.Add(currencyChange);
            statsTracker.OnCurrencyChange(currencyChange);
        }

        mainInventoryDisplay.FullUpdate(inventory);
    }
}