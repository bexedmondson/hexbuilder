using System;
using System.Collections.Generic;
using System.Text;
using Godot;

public partial class UnlockedCellPopup : Popup
{
    [Export]
    private Control infoContainer;
    
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
    private InventoryManager inventoryManager;

    private readonly Dictionary<Vector2I, string> coordsToDirectionMap = new(){
        { Vector2I.Down, "SW" },
        { Vector2I.Left, "W"},
        { Vector2I.Up, "NW"},
        { Vector2I.Right, "E"},
        { new Vector2I(1, -1), "NE"},
        { new Vector2I(-1, 1), "SE"}
    };

    public override void _Ready()
    {
        this.SetVisible(false);
        
        mapController = InjectionManager.Get<MapController>();
    }
    
    public void ShowForCell(TileMapLayer baseMapLayer, Vector2I setCell)
    {
        inventoryManager ??= InjectionManager.Get<InventoryManager>();
        confirmButton.Disabled = true;
        
        cell = setCell;
        var cellCustomTileData = baseMapLayer.GetCellTileData(cell).GetCustomData("data").Obj as CustomTileData;
        
        SetupInfo(cellCustomTileData);

        SetupTileSelector(cellCustomTileData);
        
        this.SetVisible(true);
    }

    private void SetupInfo(CustomTileData cellCustomTileData)
    {
        for (int i = infoContainer.GetChildCount() - 1; i >= 0; i--)
        {
            infoContainer.GetChild(i).QueueFree();
        }
        
        var label = new Label();
        StringBuilder sb = new StringBuilder("default:");

        foreach (var kvp in cellCustomTileData.baseTurnCurrencyChange)
        {
            sb.Append($" {kvp.Value.ToString()} {kvp.Key.ToString()}");
        }
        
        label.Text = sb.ToString();
        
        infoContainer.AddChild(label);

        sb.Clear();
        
        var adjacentCells = mapController.BaseMapLayer.GetSurroundingCells(cell);

        foreach (var adjacentCell in adjacentCells)
        {
            var adjacentData = mapController.BaseMapLayer.GetCellCustomData(adjacentCell);

            var newLabel = new Label();
            sb.Append(coordsToDirectionMap[cell - adjacentCell]);
            sb.Append($" {adjacentData.GetFileName()}");
            newLabel.Text = sb.ToString();
            infoContainer.AddChild(newLabel);
            sb.Clear();
        }

        //for each adjacent tile, check if tile data is on that tile is in adjacencies list
        //similarly check if the current tile is in the adjacencies list of that tile
    }

    private void SetupTileSelector(CustomTileData cellCustomTileData)
    {
        for (int i = tileSelector.GetChildCount() - 1; i >= 0; i--)
        {
            tileSelector.GetChild(i).QueueFree();
        }
        
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
    }

    private void OnSelectionChanged(BaseButton selectedButton)
    {
        var selectedOption = selectedButton as TileOptionUI;

        //TODO: hate that there is so much controlled from this class!! but works for now at least
        mapController.HighlightNeighbourEffects(cell, selectedOption.tileInfo);

        confirmButton.Disabled = !inventoryManager.CanAfford(new CurrencySum(selectedOption.tileInfo.tileData.price));
    }

    public override void Confirm()
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

        inventoryManager ??= InjectionManager.Get<InventoryManager>();
        inventoryManager.SpendCurrency(new CurrencySum(selectedTileInfo.tileData.price));
        Close();
    }

    public override void Close()
    {
        base.Close();
        InjectionManager.Get<MapHighlightController>().Clear();

        if (tileSelectionGroup.GetSignalConnectionList(ButtonGroup.SignalName.Pressed).Count > 0)
            tileSelectionGroup.Pressed -= OnSelectionChanged;
        
        confirmButton.Disabled = true;
    }
}