using System.Collections.Generic;
using Godot;

public partial class MainInventoryDisplay : Control
{
    [Export]
    private PackedScene singleCurrencyDisplayScene;

    [Export]
    private Color iconColor = Colors.Black;

    [Export]
    private Color negativeColor = Colors.Crimson;

    [Export]
    private Color positiveColor = Colors.LawnGreen;

    [Export]
    private Godot.Collections.Dictionary<CurrencyType, SingleCurrencyDisplay> currencyDisplays = new();
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Label> capacityLabels = new();

    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Label> currencyDeltas = new();
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, StorageCapacityIndicator> storageIndicators = new();

    private readonly string plusPrefix = "+";
    private InventoryManager inventoryManager;
    private MapCurrencyChangeAnalyser mapCurrencyChangeAnalyser;

    public void FullUpdate(CurrencySum inventory)
    {
        inventoryManager ??= InjectionManager.Get<InventoryManager>();
        mapCurrencyChangeAnalyser ??= InjectionManager.Get<MapCurrencyChangeAnalyser>();

        foreach (var kvp in currencyDisplays)
        {
            if (!inventory.ContainsKey(kvp.Key))
                GD.PushError($"[MainInventoryDisplay] No inventory currency display found for {kvp.Key}!");
            else
            {
                var amount = inventory.GetValueOrDefault(kvp.Key, 0);
                kvp.Value.SetCurrencyAmount(amount);
                kvp.Value.SetIconColor(amount < 0 ? negativeColor : iconColor);
                kvp.Value.SetTextColor(amount < 0 ? negativeColor : iconColor);
            }
        }
        
        foreach (var kvp in capacityLabels)
        {
            kvp.Value.Text = $"/ {inventoryManager.GetCurrencyCapacity(kvp.Key)}";
        }
        
        CurrencySum turnChange = mapCurrencyChangeAnalyser.GetOverallTurnDelta();

        foreach (var kvp in currencyDeltas)
        {
            UpdateCurrencyDeltaDisplay(kvp.Key, turnChange.GetValueOrDefault(kvp.Key, 0));
        }

        foreach (var kvp in storageIndicators)
        {
            var capacity = inventoryManager.GetCurrencyCapacity(kvp.Key);
            var change = turnChange.GetValueOrDefault(kvp.Key, 0);
            var current = inventory.GetValueOrDefault(kvp.Key, 0);

            if (current < capacity)
                kvp.Value.SetNotFull();
            else if (current + change > capacity)
                kvp.Value.SetOverflowing();
            else
                kvp.Value.SetFull();
        }
    }
    
    private void UpdateCurrencyDeltaDisplay(CurrencyType currencyType, int delta)
    {
        if (!currencyDeltas.TryGetValue(currencyType, out var deltaDisplay))
            return;
        
        deltaDisplay.Text = $"({(delta < 0 ? string.Empty : plusPrefix)}{delta})";

        switch (delta)
        {
            case > 0:
                deltaDisplay.SelfModulate = positiveColor;
                break;
            case < 0:
                deltaDisplay.SelfModulate = negativeColor;
                break;
            default:
                deltaDisplay.SelfModulate = iconColor;
                break;
        }
    }

    private void OnCurrencyChangePossiblyUpdated(IEvent _)
    {
        var turnChange = mapCurrencyChangeAnalyser.GetOverallTurnDelta();

        foreach (var kvp in currencyDeltas)
        {
            UpdateCurrencyDeltaDisplay(kvp.Key, turnChange.GetValueOrDefault(kvp.Key, 0));
        }
    }
}
