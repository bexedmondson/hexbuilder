using System.Collections.Generic;
using Godot;

public partial class MainInventoryDisplay : Control
{
    [Export]
    private PackedScene singleCurrencyDisplayScene;

    [Export]
    private Color iconColor = new Color(255, 255, 255);

    [Export]
    private Godot.Collections.Dictionary<CurrencyType, SingleCurrencyDisplay> currencyDisplays = new();

    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Label> currencyDeltas = new();
    
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Label> storageFullIndicators = new();

    private readonly string plusPrefix = "+";
    private MapCurrencyChangeAnalyser mapCurrencyChangeAnalyser;

    public override void _EnterTree()
    {
        base._EnterTree();

        var eventDispatcher = InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Add<MapUpdatedEvent>(OnCurrencyChangePossiblyUpdated);
        eventDispatcher.Add<WorkplaceUpdatedEvent>(OnCurrencyChangePossiblyUpdated);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        var eventDispatcher = InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Remove<MapUpdatedEvent>(OnCurrencyChangePossiblyUpdated);
        eventDispatcher.Remove<WorkplaceUpdatedEvent>(OnCurrencyChangePossiblyUpdated);
    }

    public void FullUpdate(CurrencySum inventory)
    {
        mapCurrencyChangeAnalyser ??= InjectionManager.Get<MapCurrencyChangeAnalyser>();

        foreach (var kvp in currencyDisplays)
        {
            if (!inventory.ContainsKey(kvp.Key))
                GD.PushError($"[MainInventoryDisplay] No inventory currency display found for {kvp.Key}!");
            else
                kvp.Value.SetCurrencyAmount(inventory.GetValueOrDefault(kvp.Key, 0));
        }
        
        CurrencySum turnChange = mapCurrencyChangeAnalyser.GetOverallTurnDelta();

        foreach (var kvp in currencyDeltas)
        {
            UpdateCurrencyDeltaDisplay(kvp.Key, turnChange.GetValueOrDefault(kvp.Key, 0));
        }
    }
    
    private void UpdateCurrencyDeltaDisplay(CurrencyType currencyType, int delta)
    {
        if (!currencyDeltas.TryGetValue(currencyType, out var deltaDisplay))
            return;
        
        deltaDisplay.Text = $"({(delta < 0 ? string.Empty : plusPrefix)}{delta})";
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
