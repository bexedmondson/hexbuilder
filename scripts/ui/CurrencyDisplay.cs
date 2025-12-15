using System.Collections.Generic;
using Godot;

public partial class CurrencyDisplay : Control
{
    [Export]
    protected Godot.Collections.Dictionary<CurrencyType, Resource> currencyIcons;

    [Export]
    protected PackedScene singleCurrencyDisplayScene;

    [Export]
    protected Color iconColor = new Color(255, 255, 255);

    protected Dictionary<CurrencyType, SingleCurrencyDisplay> currencyDisplays = new();

    public virtual void DisplayCurrencyAmount(CurrencySum currencySum)
    {
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
            
            existingSingleCurrencyDisplay.SetCurrency(currencyIcons[currencyType] as Texture2D, amount);
            existingSingleCurrencyDisplay.ShowDelta(false);
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