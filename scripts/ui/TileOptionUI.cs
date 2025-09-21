using System.Collections.Generic;
using Godot;

public partial class TileOptionUI : Button
{
    [Export]
    private TextureRect textureRect;

    [Export]
    private CurrencyDisplay currencyDisplay;
    
    public TileDatabase.TileInfo tileInfo { get; private set; }

    public void SetTile(TileDatabase.TileInfo setTileInfo)
    {
        tileInfo = setTileInfo;
        textureRect.Texture = tileInfo.tileTexture;

        var tilePrice = new CurrencySum(tileInfo.tileData.price);
        currencyDisplay.DisplayCurrencyAmount(tilePrice);

        var inventoryManager = InjectionManager.Get<InventoryManager>();
        this.Disabled = !inventoryManager.CanAfford(tilePrice);
    }
}