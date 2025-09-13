using System.Collections.Generic;
using Godot;

public partial class UnlockedCellPopup : Control
{
    [Export]
    private Control tileSelector;

    [Export]
    private PackedScene tileScene;

    [Export]
    private ButtonGroup tileSelectionGroup;

    [Export]
    private Button confirmButton;
    
    private Vector2I cell;

    public override void _Ready()
    {
        this.SetVisible(false);
    }
    
    public void SetCell(TileMapLayer baseMapLayer, Vector2I setCell)
    {
        cell = setCell;

        for (int i = tileSelector.GetChildCount() - 1; i >= 0; i--)
        {
            tileSelector.GetChild(i).QueueFree();
        }

        var cellCustomTileData = baseMapLayer.GetCellTileData(cell).GetCustomData("data").Obj as CustomTileData;

        TileDatabase tileDatabase = InjectionManager.Get<TileDatabase>();
        var compatibleTileInfos = tileDatabase.GetAllCompatibleTileInfos(cellCustomTileData);

        foreach (var compatibleTileInfo in compatibleTileInfos)
        {
            var tileOptionUI = tileScene.Instantiate<TileOptionUI>();
            tileOptionUI.SetTile(compatibleTileInfo);
            tileOptionUI.SetButtonGroup(tileSelectionGroup);
            tileSelector.AddChild(tileOptionUI);
        }
    }

    public void OnConfirmButton()
    {
        if (tileSelectionGroup.GetPressedButton() == null)
        {
            GD.Print("no selected button " + tileSelectionGroup.GetButtons().Count);
            return;
        }
        var selectedButton = tileSelectionGroup.GetPressedButton() as TileOptionUI;
        var selectedTileInfo = selectedButton.tileInfo;

        var mapController = InjectionManager.Get<MapController>();
        mapController.SetCell(cell, selectedTileInfo);

        var inventoryManager = InjectionManager.Get<InventoryManager>();
        inventoryManager.SpendCurrency(new Dictionary<CurrencyType, int>(selectedTileInfo.tileData.price));
        Close();
    }

    public void Close()
    {
        this.SetVisible(false);
    }
}