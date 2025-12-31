using Godot;

public partial class EncyclopediaEffectUI : Control
{
    [Export]
    private TileTextureRect tileTextureRect;

    [Export]
    private CurrencyDisplay currencyDisplay;

    [Export]
    private Control perWorkerIndicator;

    public void Setup(AdjacencyConfig adjacencyConfig)
    {
        Setup(adjacencyConfig.requiredTile, new(adjacencyConfig.currencyEffect));
    }
    
    public void Setup(CustomTileData tileData, CurrencySum currencyEffect)
    {
        tileTextureRect.SetTile(tileData);
        currencyDisplay.DisplayCurrencyAmount(currencyEffect);
        
        perWorkerIndicator.Visible = tileData.IsWorkplace;
    }
}
