using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Godot;

public partial class UnlockedCellPopup : Popup
{
    [Export]
    private Label title;
    
    [Export]
    private Control leftPanel;
    
    [Export]
    private Control effectDetailsContainer;
    
    [Export]
    private Control effectInfoParent;
    
    [Export]
    private Control workplaceDetailsContainer;
    
    [Export]
    private WorkplaceInfoUI workplaceInfoUI;

    [Export]
    private Control residentContainer;

    [Export]
    private Control residentDetailsParent;

    [Export]
    private PackedScene unlockedCellAdjacencyUIScene;
    
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

        title.Text = cellCustomTileData.GetFileName();
        
        bool hasGeneralInfo = SetupInfo(cellCustomTileData);

        workplaceDetailsContainer.Visible = cellCustomTileData.IsWorkplace;
        if (cellCustomTileData.IsWorkplace)
        {
            InjectionManager.Get<WorkplaceManager>().TryGetWorkplaceAtLocation(cell, out var workplaceState);
            workplaceInfoUI.SetWorkplaceInfo(workplaceState, null);
        }
            
        SetupResidents(cellCustomTileData);
        
        leftPanel.Visible = hasGeneralInfo || cellCustomTileData.IsWorkplace || cellCustomTileData.IsResidence;

        SetupTileSelector(cellCustomTileData);
        
        this.SetVisible(true);
    }

    private bool SetupInfo(CustomTileData cellCustomTileData)
    {
        for (int i = effectInfoParent.GetChildCount() - 1; i >= 0; i--)
        {
            effectInfoParent.GetChild(i).QueueFree();
        }
        
        var adjacentCells = mapController.BaseMapLayer.GetSurroundingCells(cell);

        bool hasEffects = false;
        
        if (cellCustomTileData.baseTurnCurrencyChange?.Count > 0)
        {
            hasEffects = true;
            var label = new Label();
            StringBuilder sb = new StringBuilder("default effects: ");

            foreach (var kvp in cellCustomTileData.baseTurnCurrencyChange)
            {
                sb.Append($" {(kvp.Value > 0 ? "+" : string.Empty)}{kvp.Value.ToString()} {kvp.Key.ToString()}");
            }

            label.Text = sb.ToString();
            effectInfoParent.AddChild(label);
        }

        foreach (var adjacentCell in adjacentCells)
        {
            var adjacentData = mapController.BaseMapLayer.GetCellCustomData(adjacentCell);
            bool hasGivenEffect = cellCustomTileData.TryGetAdjacencyEffectFromTileData(adjacentData, out var givenEffect);
            bool hasReceivedEffect = adjacentData.TryGetAdjacencyEffectFromTileData(cellCustomTileData, out var receivedEffect);

            if (!hasGivenEffect && !hasReceivedEffect)
                continue;
            
            hasEffects = true;
            
            var adjacencyEffectUI = unlockedCellAdjacencyUIScene.Instantiate<UnlockedCellAdjacencyUI>();
            adjacencyEffectUI.Setup(adjacentData, cell, adjacentCell);
            if (hasGivenEffect)
                adjacencyEffectUI.SetGivenEffects(givenEffect);
            if (hasReceivedEffect)
                adjacencyEffectUI.SetReceivedEffects(receivedEffect);
            effectInfoParent.AddChild(adjacencyEffectUI);
        }

        effectDetailsContainer.Visible = hasEffects;
        return hasEffects;
    }
    
    private void SetupResidents(CustomTileData cellCustomTileData)
    {
        if (!cellCustomTileData.IsResidence)
        {
            residentContainer.Visible = false;
            return;
        }
        
        for (int i = residentDetailsParent.GetChildCount() - 1; i >= 0; i--)
        {
            residentDetailsParent.GetChild(i).QueueFree();
        }

        residentContainer.Visible = true;

        var housingManager = InjectionManager.Get<HousingManager>();
        if (!housingManager.TryGetHouseOnCell(cell, out var houseState))
        {
            Label noneLabel = new Label();
            noneLabel.Text = "no residents";
            residentDetailsParent.AddChild(noneLabel);
            return;
        }

        foreach (var occupantData in houseState.occupants)
        {
            var residentLabel = new Label();
            residentLabel.Text = occupantData.Name;
            residentDetailsParent.AddChild(residentLabel);
        }
    }

    private void SetupTileSelector(CustomTileData cellCustomTileData)
    {
        for (int i = tileSelector.GetChildCount() - 1; i >= 0; i--)
        {
            tileSelector.GetChild(i).QueueFree();
        }
        
        TileDatabase tileDatabase = InjectionManager.Get<TileDatabase>();
        var compatibleTileInfos = tileDatabase.GetAllCompatibleTileInfos(cellCustomTileData);
        
        compatibleTileInfos.Sort((lhs, rhs) => lhs.tileData.GetFileName().CompareTo(rhs.tileData.GetFileName()));

        foreach (var compatibleTileInfo in compatibleTileInfos)
        {
            var tileOptionUI = tileScene.Instantiate<TileOptionUI>();
            tileOptionUI.SetTile(compatibleTileInfo);
            tileOptionUI.SetButtonGroup(tileSelectionGroup);
            tileSelector.AddChild(tileOptionUI);
        }
        
        //TODO add locked options as well

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
        mapController.BuildTileAtCell(cell, selectedTileInfo);

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

        foreach (var residentInfo in residentDetailsParent.GetChildren())
        {
            residentInfo.QueueFree();
        }
        
        foreach (var effectInfo in effectInfoParent.GetChildren())
        {
            effectInfo.QueueFree();
        }

        foreach (var tileSelectionInfo in tileSelector.GetChildren())
        {
            tileSelectionInfo.QueueFree();
        }
        
        confirmButton.Disabled = true;
    }
}