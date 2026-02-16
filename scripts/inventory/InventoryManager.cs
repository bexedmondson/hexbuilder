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

    private CurrencySum capacities = new();

    private InventoryStatsTracker statsTracker;

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);

        statsTracker = new InventoryStatsTracker();
        
        var eventDispatcher = InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Add<MapUpdatedEvent>(OnPotentialInventoryChange);
        eventDispatcher.Add<WorkplaceUpdatedEvent>(OnPotentialInventoryChange);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        InjectionManager.Deregister(this);
        statsTracker = null;
        
        var eventDispatcher = InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Remove<MapUpdatedEvent>(OnPotentialInventoryChange);
        eventDispatcher.Remove<WorkplaceUpdatedEvent>(OnPotentialInventoryChange);
    }

    public void OnNewGame()
    {
        inventory.Clear();
        inventory = new CurrencySum(startAmounts);
        
        mainInventoryDisplay.FullUpdate(inventory);
    }

    public Texture2D GetIcon(CurrencyType currencyType)
    {
        return currencyIcons[currencyType];
    }

    public int GetCurrentCurrencyQuantity(CurrencyType currencyType)
    {
        return inventory.GetValueOrDefault(currencyType, 0);
    }

    public int GetCurrencyCapacity(CurrencyType currencyType)
    {
        return capacities.GetValueOrDefault(currencyType, 0);
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

    public void AwardCurrency(CurrencySum price)
    {
        inventory.Add(price);
        mainInventoryDisplay.FullUpdate(inventory);
    }
    
    public void SpendCurrency(CurrencySum price)
    {
        inventory.Subtract(price);
        
        mainInventoryDisplay.FullUpdate(inventory);
    }

    public void OnNextTurn(CurrencySum[] allCurrencyChanges)
    {
        //basically trying to be nice to the player here and give the maximum usage and creation stats, so
        //need to do all the minusing before all the adding now that i have the storage limits, so that out-of-order
        //storage capacity discards don't interfere

        CurrencySum totalMinus = new();
        CurrencySum totalPlus = new();

        foreach (var currencyChange in allCurrencyChanges)
        {
            foreach (var individualCurrencyChange in currencyChange)
            {
                switch (individualCurrencyChange.Value)
                {
                    case 0:
                        continue;
                    case < 0:
                        totalMinus.Add(individualCurrencyChange.Key, individualCurrencyChange.Value);
                        break;
                    default:
                        totalPlus.Add(individualCurrencyChange.Key, individualCurrencyChange.Value);
                        break;
                }
            }
        }

        inventory.Add(totalMinus);
        statsTracker.OnCurrencyChange(totalMinus);

        var actualAdd = new CurrencySum();
        //capping increases at capacity limits
        foreach (var individualCurrencyAdd in totalPlus)
        {
            var potentialFinalValue = inventory.GetValueOrDefault(individualCurrencyAdd.Key, 0) + individualCurrencyAdd.Value;
            var cappedFinalValue = Mathf.Min(potentialFinalValue, capacities.GetValueOrDefault(individualCurrencyAdd.Key, 0));
            var finalDelta = cappedFinalValue - inventory.GetValueOrDefault(individualCurrencyAdd.Key, 0);
            actualAdd.Add(individualCurrencyAdd.Key, finalDelta);
        }
        
        inventory.Add(actualAdd);
        statsTracker.OnCurrencyChange(actualAdd);
        
        //don't track stuff lost due to overflow, so discard overflows here
        foreach (var kvp in inventory)
        {
            var capacity = capacities.GetValueOrDefault(kvp.Key, 0);
            if (capacity < kvp.Value)
            {
                int diff = capacity - kvp.Value;
                GD.Print($"[InventoryManager] discarding overflow {diff} {kvp.Key}");
                inventory.Add(kvp.Key, diff); //diff will be negative so adding will set to the correct values
            }
        }
        
        //send notifications
        var toastManager = InjectionManager.Get<ToastManager>();
        foreach (var add in actualAdd)
        {
            if (add.Value != 0)
                toastManager.RequestToast(new ToastConfig{text = $"+{add.Value}", stackId = $"{add.Key.ToString().ToLower()}_add"});;
        }
        foreach (var minus in totalMinus)
        {
            if (minus.Value != 0)
                toastManager.RequestToast(new ToastConfig{text = $"{minus.Value}", stackId = $"{minus.Key.ToString().ToLower()}_minus"});
        }
        
        /*foreach (var currencyChange in allCurrencyChanges)
        {
            foreach (var kvp in currencyChange)
            {
                if (capacities.GetValueOrDefault(kvp.Key, 0) <= inventory.GetValueOrDefault(kvp.Key))
                    //do something
                    GD.Print("hi");
            }
            
            inventory.Add(currencyChange);
            
            statsTracker.OnCurrencyChange(currencyChange);
        }*/

        mainInventoryDisplay.FullUpdate(inventory);
    }

    private void OnPotentialInventoryChange(IEvent e)
    {
        var storageAnalyser = InjectionManager.Get<MapStorageAnalyser>();
        capacities = storageAnalyser.GetTotalStorage();
        
        mainInventoryDisplay.FullUpdate(inventory);
    }
}