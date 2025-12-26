
using Godot;

public partial class SingleCurrencyDisplay : Control
{
    [Export]
    private TextureRect icon;

    [Export]
    private Label quantityLabel;

    private readonly string plusPrefix = "+";
    private readonly string minusPrefix = "-";

    public void SetCurrencyIcon(CurrencyType currencyType)
    {
        var inventoryManager = InjectionManager.Get<InventoryManager>();
        icon.Texture = inventoryManager.GetIcon(currencyType);
    }
    
    public void SetCurrencyAmount(int quantity, bool displayAsDelta = false)
    {
        quantityLabel.Text = $"{(displayAsDelta && quantity > 0 ? plusPrefix : string.Empty)}{quantity}";
    }

    public void SetIconColor(Color color)
    {
        icon.SelfModulate = color;
    }
}
