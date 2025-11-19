
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

    public void SetCurrency(Texture2D iconTexture, int quantity, int delta = 0)
    {
        icon.Texture = iconTexture;
        quantityLabel.Text = quantity.ToString();
        SetDelta(delta);
    }

    public void SetIconColor(Color color)
    {
        icon.SelfModulate = color;
    }

    public void ShowDelta(bool showDelta)
    {
        changeLabel.Visible = showDelta;
    }

    public void SetDelta(int delta)
    {
        changeLabel.Text = $"({(delta < 0 ? minusPrefix : plusPrefix)}{delta})";
    }
}
