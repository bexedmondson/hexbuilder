using System.Text;
using Godot;

public partial class UnlockedCellPopup : Popup
{
    [Export]
    private Label title;

    [Export]
    private TabContainer tabContainer;

    [Export]
    private Control effectsTab;

    [Export]
    private Control residentTab;

    [Export]
    private Control workerTab;

    [Export]
    private Control storageTab;

    [Export]
    private Control buildTab;
    
    
    [Export]
    private Control effectInfoParent;

    [Export]
    private PackedScene unlockedCellAdjacencyUIScene;

    [Export]
    private StorageInfoUI storageInfoUI;

    [Export]
    private Control residentDetailsParent;
    
    [Export]
    private WorkplaceInfoUI workplaceInfoUI;
    
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
        
        SetupInfo(cellCustomTileData);

        tabContainer.SetTabHidden(workerTab.GetIndex(), !cellCustomTileData.IsWorkplace);
        if (cellCustomTileData.IsWorkplace)
        {
            InjectionManager.Get<WorkplaceManager>().TryGetWorkplaceAtLocation(cell, out var workplaceState);
            workplaceInfoUI.SetWorkplaceInfo(workplaceState, null);
        }

        if (cellCustomTileData.TryGetComponent<StorageCapacityDataComponent>(out var storageComponent))
        {
            storageInfoUI.SetStorageInfo(storageComponent);
            tabContainer.SetTabHidden(storageTab.GetIndex(), false);
        }
        else
        {
            tabContainer.SetTabHidden(storageTab.GetIndex(), true);
        }
            
        SetupResidents(cellCustomTileData);

        SetupTileSelector(cellCustomTileData);
        
        this.SetVisible(true);
    }

    private void SetupInfo(CustomTileData cellCustomTileData)
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

        tabContainer.SetTabHidden(effectsTab.GetIndex(), !hasEffects);
    }
    
    private void SetupResidents(CustomTileData cellCustomTileData)
    {
        if (!cellCustomTileData.IsResidence)
        {
            tabContainer.SetTabHidden(residentTab.GetIndex(), true);
            return;
        }
        
        for (int i = residentDetailsParent.GetChildCount() - 1; i >= 0; i--)
        {
            residentDetailsParent.GetChild(i).QueueFree();
        }

        tabContainer.SetTabHidden(residentTab.GetIndex(), false);

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
        
        tabContainer.SetTabHidden(buildTab.GetIndex(), false); //TODO change?
        
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

        confirmButton.Disabled = !inventoryManager.CanAfford(new CurrencySum(selectedOption.tileInfo.tileData.buildPrice));
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
        inventoryManager.SpendCurrency(new CurrencySum(selectedTileInfo.tileData.buildPrice));
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