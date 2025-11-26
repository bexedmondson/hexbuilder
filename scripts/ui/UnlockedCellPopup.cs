using System;
using System.Collections.Generic;
using Godot;

public partial class UnlockedCellPopup : Popup
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

    private MapController mapController;

    public override void _Ready()
    {
        this.SetVisible(false);
        
        mapController = InjectionManager.Get<MapController>();
    }
    
    public void ShowForCell(TileMapLayer baseMapLayer, Vector2I setCell)
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

        tileSelectionGroup.Pressed += OnSelectionChanged;
        
        this.SetVisible(true);
    }

    private void OnSelectionChanged(BaseButton selectedButton)
    {
        var selectedOption = selectedButton as TileOptionUI;

        //TODO: hate that there is so much controlled from this class!! but works for now at least
        mapController.HighlightNeighbourEffects(cell, selectedOption.tileInfo);
    }

    public override void Confirm ()
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
        inventoryManager.SpendCurrency(new CurrencySum(selectedTileInfo.tileData.price));
        Close();
    }

    public override void Close()
    {
        base.Close();
        InjectionManager.Get<MapHighlightController>().Clear();

        if (tileSelectionGroup.GetSignalConnectionList(ButtonGroup.SignalName.Pressed).Count > 0)
            tileSelectionGroup.Pressed -= OnSelectionChanged;
    }
}