using System.Text;
using Godot;

public partial class UnlockedCellPopup : Popup
{
    [Export]
    private Label title;

    [Export]
    private Control selectedCellInfoButton;

    [ExportGroup("Tabs")]
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

    [ExportGroup("Resources Tab")]
    [Export]
    private UnlockedCellPopup_Resources resourcesDisplay;
    
    [Export]
    private Control effectInfoParent;

    [Export]
    private CurrencyDisplay defaultEffectDisplay;

    [Export]
    private PackedScene unlockedCellAdjacencyUIScene;

    [Export]
    private Control maxBonusContainer;

    [Export]
    private CurrencyDisplay maxBonusCurrencyDisplay;

    [ExportGroup("Storage Tab")]
    [Export]
    private StorageInfoUI storageInfoUI;

    [ExportGroup("Residents Tab")]
    [Export]
    private Control residentDetailsParent;
    
    [ExportGroup("Workers Tab")]
    [Export]
    private WorkplaceInfoUI workplaceInfoUI;
    
    [ExportGroup("BuildTab")]
    [Export]
    private Control tileSelector;

    [Export]
    private PackedScene tileScene;

    [Export]
    private ButtonGroup tileSelectionGroup;

    [Export]
    private EncyclopediaPopup encyclopediaPopup;

    [Export]
    private Button confirmButton;
    
    private Vector2I cell = Vector2I.MinValue;
    private CustomTileData cellCustomTileData;

    private MapController mapController;
    private InventoryManager inventoryManager;
    private WorkplaceManager workplaceManager;

    public override void _Ready()
    {
        this.SetVisible(false);
        
        mapController = InjectionManager.Get<MapController>();
        selectedCellInfoButton.Visible = false;
    }
    
    public void ShowForCell(TileMapLayer baseMapLayer, Vector2I setCell)
    {
        if (setCell == cell)
            return;
        
        inventoryManager ??= InjectionManager.Get<InventoryManager>();
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        confirmButton.Disabled = true;
        
        cell = setCell;
        cellCustomTileData = baseMapLayer.GetCellTileData(cell).GetCustomData("data").Obj as CustomTileData;
        
        TileDatabase tileDatabase = InjectionManager.Get<TileDatabase>();
        selectedCellInfoButton.Visible = tileDatabase.AllBuildingTileInfos.Exists(tileInfo => tileInfo.tileData == cellCustomTileData);

        title.Text = cellCustomTileData.GetFileName();
        
        SetupInfo();

        tabContainer.SetTabHidden(workerTab.GetIndex(), !cellCustomTileData.IsWorkplace);
        if (cellCustomTileData.IsWorkplace)
        {
            workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplaceState);
            workplaceInfoUI.SetWorkplaceInfo(workplaceState, null);
            
            InjectionManager.Get<EventDispatcher>().Add<WorkplaceUpdatedEvent>(OnWorkplaceUpdated);
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
            
        SetupResidents();

        SetupTileSelector();

        tabContainer.SetCurrentTab(0);
        if (tabContainer.IsTabHidden(0))
            tabContainer.SelectNextAvailable();
        this.SetVisible(true);
    }

    private void SetupInfo()
    {
        resourcesDisplay.Setup(cellCustomTileData, cell, out bool hasAnyEffects);
        tabContainer.SetTabHidden(effectsTab.GetIndex(), !hasAnyEffects);
    }

    private void SetupMaxBonus(CustomTileData tileData)
    {
        bool hasMaxBonus = tileData.TryGetComponent(out MaximumWorkerProductionBonusComponent maxBonusComponent);
        workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplaceState);
        maxBonusContainer.Visible = hasMaxBonus && workplaceState.workerCount >= workplaceState.capacity;
        if (!hasMaxBonus) 
            return;
        
        maxBonusCurrencyDisplay.DisplayCurrencyAmount(new CurrencySum(maxBonusComponent.extraBaseProduction));
    }

    private void OnWorkplaceUpdated(WorkplaceUpdatedEvent e)
    {
        workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplaceState);
        if (!e.newOrChangedWorkplaces.Contains(workplaceState))
            return;
        
        resourcesDisplay.Setup(cellCustomTileData, cell, out _);
    }
    
    private void SetupResidents()
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

    private void SetupTileSelector()
    {
        for (int i = tileSelector.GetChildCount() - 1; i >= 0; i--)
        {
            tileSelector.GetChild(i).QueueFree();
        }
        
        tabContainer.SetTabHidden(buildTab.GetIndex(), false); //TODO change?
        
        TileDatabase tileDatabase = InjectionManager.Get<TileDatabase>();
        var compatibleTileInfos = tileDatabase.GetAllCompatibleTileInfos(cellCustomTileData);
        
        compatibleTileInfos.Sort((lhs, rhs) =>
        {
            var lockedCompare = rhs.tileData.IsUnlocked().CompareTo(lhs.tileData.IsUnlocked());
            if (lockedCompare == 0)
                lockedCompare = lhs.tileData.GetFileName().CompareTo(rhs.tileData.GetFileName());

            return lockedCompare;
        });

        foreach (var compatibleTileInfo in compatibleTileInfos)
        {
            if (compatibleTileInfo.tileData.TryGetComponent(out HideInGameComponent _))
                continue;
            
            var tileOptionUI = tileScene.Instantiate<TileOptionUI>();
            tileOptionUI.SetTile(compatibleTileInfo);
            tileOptionUI.SetButtonGroup(tileSelectionGroup);
            tileOptionUI.SetupInfoButton(true, OnInfoButton);
            tileSelector.AddChild(tileOptionUI);
        }

        if (tileSelectionGroup.GetSignalConnectionList(ButtonGroup.SignalName.Pressed).Count == 0)
            tileSelectionGroup.Pressed += OnSelectionChanged;
    }

    public void OnSelectedCellInfoButton()
    {
        encyclopediaPopup.ShowPopup(cellCustomTileData);
    }

    public void OnInfoButton(CustomTileData tileDataToShow)
    {
        encyclopediaPopup.ShowPopup(tileDataToShow);
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
        
        InjectionManager.Get<EventDispatcher>().Remove<WorkplaceUpdatedEvent>(OnWorkplaceUpdated);
        
        cell = Vector2I.MinValue;

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