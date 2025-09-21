using System.Collections.Generic;
using Godot;

public partial class CurrencyDisplay : Control
{
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Resource> currencyIcons;

    public void DisplayCurrencyAmount(CurrencySum currencySum)
    {
        foreach (var kvp in currencySum)
        {
            var currencyType = kvp.Key;
            var amount = kvp.Value;
            
            var textureRect = new TextureRect();
            textureRect.Texture = currencyIcons[currencyType] as Texture2D;
            textureRect.CustomMinimumSize = Vector2.One * 40;
            textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional;
            this.AddChild(textureRect);

            var label = new Label();
            label.LabelSettings = new LabelSettings() { FontSize = 25 };
            label.Text = amount.ToString() + "   ";
            this.AddChild(label);
        }
    }

    public void Cleanup()
    {
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            this.GetChild(i).QueueFree();
        }
    }
}