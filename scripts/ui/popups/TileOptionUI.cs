using System;
using System.Collections.Generic;
using Godot;

public partial class TileOptionUI : Button
{
    [Export]
    private TileTextureRect textureRect;

    [Export]
    private Label nameLabel;

    [Export]
    private CurrencyDisplay currencyDisplay;
    
    [Export]
    private Control lockIndicator;

    [Export]
    private Button infoButton;

    private Action<CustomTileData> OnInfoButtonCallback;
    
    public TileDatabase.TileInfo tileInfo { get; private set; }

    public virtual void SetTile(TileDatabase.TileInfo setTileInfo)
    {
        tileInfo = setTileInfo;
        nameLabel.Text = setTileInfo.tileData.GetFileName();
        textureRect.SetTile(tileInfo);
        
        if (lockIndicator != null)
            lockIndicator.Visible = !tileInfo.tileData.IsUnlocked();

        SetCostState();
    }

    protected virtual void SetCostState()
    {
        var tilePrice = new CurrencySum(tileInfo.tileData.buildPrice);
        currencyDisplay.DisplayCurrencyAmount(tilePrice);
        
        var inventoryManager = InjectionManager.Get<InventoryManager>();
        this.Disabled = !inventoryManager.CanAfford(tilePrice) || !tileInfo.tileData.IsUnlocked();
    }

    public void SetupInfoButton(bool visible, Action<CustomTileData> action = null)
    {
        infoButton.Visible = visible;
        OnInfoButtonCallback = action;
    }

    public void OnInfoButton()
    {
        OnInfoButtonCallback?.Invoke(tileInfo.tileData);
    }
}