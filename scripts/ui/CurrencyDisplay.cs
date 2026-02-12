using System.Collections.Generic;
using Godot;

public partial class CurrencyDisplay : Control
{
    [Export]
    private PackedScene singleCurrencyDisplayScene;

    [Export]
    private bool displayAsDelta = false;

    [Export]
    private Color iconColor = Colors.Black;

    private Dictionary<CurrencyType, SingleCurrencyDisplay> currencyDisplays = new();

    public virtual void DisplayCurrencyAmount(IDictionary<CurrencyType, int> currencySum)
    {
        foreach (var kvp in currencySum)
        {
            DisplaySingleCurrency(kvp.Key, kvp.Value);
        }

        List<CurrencyType> toRemove = new();
        foreach (var kvp in currencyDisplays)
        {
            if (!currencySum.ContainsKey(kvp.Key))
            {
                kvp.Value.QueueFree();
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var currencyTypeToRemove in toRemove)
        {
            currencyDisplays.Remove(currencyTypeToRemove);
        }
    }

    private void DisplaySingleCurrency(CurrencyType currencyType, int amount)
    {
        var hasDisplay = currencyDisplays.TryGetValue(currencyType, out var existingSingleCurrencyDisplay);

        if (!hasDisplay)
        {
            existingSingleCurrencyDisplay = singleCurrencyDisplayScene.Instantiate<SingleCurrencyDisplay>();
            existingSingleCurrencyDisplay.SetIconColor(iconColor);
            currencyDisplays[currencyType] = existingSingleCurrencyDisplay;
            this.AddChild(existingSingleCurrencyDisplay);
        }
            
        existingSingleCurrencyDisplay.SetCurrencyIcon(currencyType);
        existingSingleCurrencyDisplay.SetCurrencyAmount(amount, displayAsDelta);
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