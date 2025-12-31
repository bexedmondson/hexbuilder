using Godot;

public partial class EncyclopediaPopup : Popup
{
    [Export]
    private Control tileSelector;

    [Export]
    private PackedScene tileScene;

    [Export]
    private Control tileDetails;

    [Export]
    private ButtonGroup encyclopediaTileGroup;

    [Export]
    private TileTextureRect tileTextureRect;

    [Export]
    private CurrencyDisplay defaultEffectDisplay;

    [Export]
    private Control affectsParent;

    [Export]
    private Control affectedByParent;
    
    [Export]
    private PackedScene affectedByScene;
    
    public override void _Ready()
    {
        base._Ready();
        this.Visible = false;
    }

    public void ShowPopup()
    {
        for (int i = tileSelector.GetChildCount() - 1; i >= 0; i--)
        {
            tileSelector.GetChild(i).QueueFree();
        }
        tileDetails.Visible = false;
        
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
            tileSelector.AddChild(tileOptionUI);
        }

        encyclopediaTileGroup.Pressed += OnSelectionChanged;
        this.Visible = true;
    }

    private void OnSelectionChanged(BaseButton selectedButton)
    {
        tileDetails.Visible = true;
        
        var selectedOption = selectedButton as TileOptionUI;
        tileTextureRect.SetTile(selectedOption.tileInfo);
        
        defaultEffectDisplay.DisplayCurrencyAmount(new CurrencySum(selectedOption.tileInfo.tileData.baseTurnCurrencyChange));

        SetupAffects(selectedOption.tileInfo.tileData);

        SetupAffectedBy(selectedOption.tileInfo.tileData);
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

    public override void Close()
    {
        base.Close();
        if (encyclopediaTileGroup.GetSignalConnectionList(ButtonGroup.SignalName.Pressed).Count > 0)
            encyclopediaTileGroup.Pressed -= OnSelectionChanged;
    }
}
