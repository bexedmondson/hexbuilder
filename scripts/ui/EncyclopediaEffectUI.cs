using Godot;

public partial class EncyclopediaEffectUI : Control
{
    [Export]
    private TileTextureRect tileTextureRect;

    [Export]
    private CurrencyDisplay currencyDisplay;

    public void Setup(AdjacencyConfig adjacencyConfig)
    {
        tileTextureRect.SetTile(adjacencyConfig.requiredTile);
        currencyDisplay.DisplayCurrencyAmount(new CurrencySum(adjacencyConfig.currencyEffect));
    }
    
    public void Setup(CustomTileData tileData, CurrencySum currencyEffect)
    {
        tileTextureRect.SetTile(tileData);
        currencyDisplay.DisplayCurrencyAmount(currencyEffect);
    }
}
