using Godot;

public partial class EncyclopediaPopup : Popup
{
    [Export]
    private TileSelectorList tileSelector;

    [Export]
    private PackedScene tileScene;

    [Export]
    private Label tileNameLabel;

    [Export]
    private Control tileDetailsExpanders;
    
    [Export]
    private Control tileDetailsInfo;

    [Export]
    private ButtonGroup encyclopediaTileGroup;

    [Export]
    private TileTextureRect tileTextureRect;

    [Export]
    private CurrencyDisplay defaultEffectDisplay;

    [Export]
    private Control workerInfoContainer;

    [Export]
    private Texture2D workerIconTexture;

    [Export]
    private Control workerCountContainer;

    [Export]
    private Control maxBonusContainer;

    [Export]
    private CurrencyDisplay maxBonusCurrencyDisplay;

    [Export]
    private Control unlockReqParent;
    
    [Export]
    private Control unlockReqContainer;

    [Export]
    private Control buildPriceContainer;

    [Export]
    private CurrencyDisplay buildPriceDisplay;

    [Export]
    private Control affectsParent;

    [Export]
    private Control affectedByParent;
    
    [Export]
    private PackedScene affectedByScene;

    [Export]
    private StorageInfoUI storageInfoUI;
    
    public override void _Ready()
    {
        base._Ready();
        this.Visible = false;
    }

    public void ShowPopup()
    {
        tileSelector.Cleanup();
        
        tileDetailsExpanders.Visible = false;
        tileDetailsInfo.Visible = false;
        
        TileDatabase tileDatabase = InjectionManager.Get<TileDatabase>();
        var allBuildingTileInfos = tileDatabase.AllBuildingTileInfos;
        
        allBuildingTileInfos.Sort((lhs, rhs) => lhs.tileData.GetFileName().CompareTo(rhs.tileData.GetFileName()));

        foreach (var tileInfo in allBuildingTileInfos)
        {
            if (tileInfo.tileData.TryGetComponent<HideInEncyclopediaComponent>(out _))
                continue;
            
            var tileOptionUI = tileScene.Instantiate<TileOptionUI>();
            tileOptionUI.SetTile(tileInfo);
            tileOptionUI.SetButtonGroup(encyclopediaTileGroup);
            tileSelector.AddTileOptionUI(tileOptionUI);
        }

        encyclopediaTileGroup.Pressed += OnSelectionChanged;
        this.Visible = true;
    }

    public void ShowPopup(CustomTileData tileToShow)
    {
        ShowPopup();

        var tileOptionUIs = tileSelector.GetTileOptionUIs();
        foreach (var tileOptionUI in tileOptionUIs)
        {
            if (tileOptionUI.tileInfo.tileData == tileToShow)
                tileOptionUI.SetPressed(true);
        }
    }

    private void OnSelectionChanged(BaseButton selectedButton)
    {
        tileDetailsExpanders.Visible = true;
        tileDetailsInfo.Visible = true;
        
        var selectedOption = selectedButton as TileOptionUI;
        tileTextureRect.SetTile(selectedOption.tileInfo);
        var selectedTileData = selectedOption.tileInfo.tileData;

        tileNameLabel.Text = selectedTileData.GetFileName();
        defaultEffectDisplay.DisplayCurrencyAmount(new CurrencySum(selectedTileData.baseTurnCurrencyChange));

        SetupWorkerCountDisplay(selectedTileData);
        SetupMaxBonus(selectedTileData);

        SetupUnlockRequirements(selectedTileData);

        SetupBuildCosts(selectedTileData);
        
        SetupAffects(selectedTileData);
        SetupAffectedBy(selectedTileData);

        SetupStorageInfo(selectedTileData);
    }

    private void SetupWorkerCountDisplay(CustomTileData tileData)
    {
        bool hasWorkerCapacity = tileData.TryGetComponent(out WorkerCapacityComponent workerCapacity) && workerCapacity.capacity > 0;
        workerInfoContainer.Visible = hasWorkerCapacity;
        
        if (!hasWorkerCapacity)
            return;
        
        for (int i = workerCountContainer.GetChildCount() - 1; i >= workerCapacity.capacity; i--)
        {
            workerCountContainer.GetChild(i).QueueFree();
        }

        for (int i = workerCountContainer.GetChildCount() - 1; i < workerCapacity.capacity; i++)
        {
            TextureRect workerIcon = new TextureRect();
            workerIcon.Texture = workerIconTexture;
            workerIcon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            workerIcon.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
            workerIcon.CustomMinimumSize = Vector2I.One * 27;
            workerIcon.SelfModulate = new Color("4c4c4c");
            workerCountContainer.AddChild(workerIcon);
        }
    }

    private void SetupMaxBonus(CustomTileData tileData)
    {
        bool hasMaxBonus = tileData.TryGetComponent(out MaximumWorkerProductionBonusComponent maxBonusComponent);
        maxBonusContainer.Visible = hasMaxBonus;
        if (!hasMaxBonus)
            return;
        
        maxBonusCurrencyDisplay.DisplayCurrencyAmount(new CurrencySum(maxBonusComponent.extraBaseProduction));
    }

    private void SetupUnlockRequirements(CustomTileData tileData)
    {
        for (int i = unlockReqContainer.GetChildCount() - 1; i >= 0; i--)
        {
            unlockReqContainer.GetChild(i).QueueFree();
        }

        if (tileData.IsUnlocked() || !tileData.TryGetComponent(out UnlockRequirementsComponent unlockRequirementsComponent))
        {
            unlockReqParent.Visible = false;
            return;
        }

        unlockReqParent.Visible = true;
        var factory = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        foreach (var requirement in unlockRequirementsComponent.requirements)
        {
            var requirementUI = factory.CreateUIInstanceForRequirement(requirement);
            requirementUI.Setup(requirement, tileData);
            unlockReqContainer.AddChild(requirementUI);
        }
    }

    private void SetupBuildCosts(CustomTileData tileData)
    {
        if (tileData.buildPrice != null && tileData.buildPrice.Count > 0)
        {
            buildPriceContainer.Visible = true;
            buildPriceDisplay.DisplayCurrencyAmount(new CurrencySum(tileData.buildPrice));
        }
        else
        {
            buildPriceContainer.Visible = false;
        }
    }

    private void SetupAffects(CustomTileData tileData)
    {
        for (int i = affectsParent.GetChildCount() - 1; i >= 0; i--)
        {
            affectsParent.GetChild(i).QueueFree();
        }
        
        TileDatabase tileDatabase = InjectionManager.Get<TileDatabase>();
        var allBuildingTileInfos = tileDatabase.AllBuildingTileInfos;

        foreach (var otherTileInfo in allBuildingTileInfos)
        {
            foreach (var otherAdjacency in otherTileInfo.tileData.adjacencies)
            {
                if (otherAdjacency.requiredTile != tileData)
                    continue;
                
                var affectsUI = affectedByScene.Instantiate<EncyclopediaEffectUI>();
                affectsUI.Setup(otherTileInfo.tileData, new CurrencySum(otherAdjacency.currencyEffect));
                affectsParent.AddChild(affectsUI);
            }
        }
    }

    private void SetupAffectedBy(CustomTileData tileData)
    {
        for (int i = affectedByParent.GetChildCount() - 1; i >= 0; i--)
        {
            affectedByParent.GetChild(i).QueueFree();
        }

        foreach (var adjacency in tileData.adjacencies)
        {
            var affectedByUI = affectedByScene.Instantiate<EncyclopediaEffectUI>();
            affectedByUI.Setup(adjacency);
            affectedByParent.AddChild(affectedByUI);
        }
    }

    private void SetupStorageInfo(CustomTileData tileData)
    {
        if (tileData.TryGetComponent<StorageCapacityDataComponent>(out var storageComponent))
        {
            storageInfoUI.Visible = true;
            storageInfoUI.SetStorageInfo(storageComponent);
        }
        else
        {
            storageInfoUI.Visible = false;
        }
    }

    public override void Close()
    {
        base.Close();
        if (encyclopediaTileGroup.GetSignalConnectionList(ButtonGroup.SignalName.Pressed).Count > 0)
            encyclopediaTileGroup.Pressed -= OnSelectionChanged;
    }
}
