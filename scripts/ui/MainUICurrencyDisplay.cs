using System.Collections.Generic;
using Godot;

public partial class MainUICurrencyDisplay : CurrencyDisplay
{
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

    public override void DisplayCurrencyAmount(CurrencySum currencySum)
    {
        mapCurrencyChangeAnalyser ??= InjectionManager.Get<MapCurrencyChangeAnalyser>();
        CurrencySum turnChange = mapCurrencyChangeAnalyser.GetOverallTurnDelta();
        
        foreach (var kvp in currencySum)
        {
            var currencyType = kvp.Key;
            var amount = kvp.Value;
            
            var hasDisplay = currencyDisplays.TryGetValue(currencyType, out var existingSingleCurrencyDisplay);

            if (!hasDisplay)
            {
                existingSingleCurrencyDisplay = singleCurrencyDisplayScene.Instantiate<SingleCurrencyDisplay>();
                existingSingleCurrencyDisplay.SetIconColor(iconColor);
                currencyDisplays[currencyType] = existingSingleCurrencyDisplay;
                this.AddChild(existingSingleCurrencyDisplay);
            }

            int delta = 0;
            turnChange.TryGetValue(kvp.Key, out delta);
            existingSingleCurrencyDisplay.SetCurrency(currencyIcons[currencyType] as Texture2D, amount, delta);
            existingSingleCurrencyDisplay.ShowDelta(true);
        }
    }

    private void OnCurrencyChangePossiblyUpdated(IEvent _)
    {
        var turnDelta = mapCurrencyChangeAnalyser.GetOverallTurnDelta();

        foreach (var kvp in currencyDisplays)
        {
            kvp.Value.SetDelta(turnDelta.GetValueOrDefault(kvp.Key, 0));
        }
    }

    public void Cleanup()
    {
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            this.GetChild(i).QueueFree();
        }
        
        currencyDisplays.Clear();
    }
}
