using System.Collections.Generic;
using Godot;

public partial class CurrencyDisplay : Control
{
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Resource> currencyIcons;

    [Export]
    private bool showTurnChange = false;

    [Export]
    private PackedScene singleCurrencyDisplayScene;

    [Export]
    private Color iconColor = new Color(255, 255, 255);

    private MapCurrencyChangeAnalyser mapCurrencyChangeAnalyser;

    private Dictionary<CurrencyType, SingleCurrencyDisplay> currencyDisplays = new();

    public override void _EnterTree()
    {
        base._EnterTree();

        if (showTurnChange)
        {
            var eventDispatcher = InjectionManager.Get<EventDispatcher>();
            eventDispatcher.Add<MapUpdatedEvent>(OnMapUpdated);
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if (showTurnChange)
        {
            var eventDispatcher = InjectionManager.Get<EventDispatcher>();
            eventDispatcher.Remove<MapUpdatedEvent>(OnMapUpdated);
        }
    }

    public void DisplayCurrencyAmount(CurrencySum currencySum)
    {
        CurrencySum turnChange = new();
        if (showTurnChange)
        {
            mapCurrencyChangeAnalyser ??= InjectionManager.Get<MapCurrencyChangeAnalyser>();
            turnChange = mapCurrencyChangeAnalyser.GetOverallTurnDelta();
        }
        
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

            if (showTurnChange)
            {
                int delta = 0;
                turnChange.TryGetValue(kvp.Key, out delta);
                existingSingleCurrencyDisplay.SetCurrency(currencyIcons[currencyType] as Texture2D, amount, delta);
                existingSingleCurrencyDisplay.ShowDelta(true);
            }
            else
            {
                existingSingleCurrencyDisplay.SetCurrency(currencyIcons[currencyType] as Texture2D, amount);
                existingSingleCurrencyDisplay.ShowDelta(false);
            }
        }
    }

    private void OnMapUpdated(MapUpdatedEvent mapUpdatedEvent)
    {
        if (!showTurnChange)
            return;

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