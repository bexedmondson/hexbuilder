
using Godot;

public partial class SingleCurrencyDisplay : Control
{
    [Export]
    private TextureRect icon;

    [Export]
    private Label quantityLabel;

    [Export]
    private Label changeLabel;

    private readonly string plusPrefix = "+";
    private readonly string minusPrefix = "-";

    public void SetCurrency(Texture2D iconTexture, int quantity, bool displayAsDelta = false)
    {
        icon.Texture = iconTexture;
        quantityLabel.Text = $"{(displayAsDelta && quantity > 0 ? "+" : string.Empty)}{quantity}";
    }

    public void SetIconColor(Color color)
    {
        icon.SelfModulate = color;
    }

    public void ShowSeparateDelta(bool showDelta, int delta = 0)
    {
        changeLabel.Visible = showDelta;
        
        if (showDelta)
            changeLabel.Text = $"({(delta < 0 ? string.Empty : plusPrefix)}{delta})";
    }
}
