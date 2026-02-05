using Godot;

public partial class UnlockedCellPopup_AutoUpgrade : Control
{
    [Export]
    private TileTextureRect upgradeResultTile;

    [Export]
    private Control upgradeRequirementsContainer;

    [Export]
    private Control upgradeRequirementsParent;

    private MapController mapController;

    private CustomTileData selectedCellTileData;
    private Vector2I selectedCell;

    public override void _Ready()
    {
        base._Ready();
        mapController = InjectionManager.Get<MapController>();
    }

    public void Setup(CustomTileData cellTileData, Vector2I cell)
    {
        Cleanup();

        if (!cellTileData.TryGetComponent(out AutoUpgradeComponent autoUpgradeComponent))
            return;
        
        this.selectedCellTileData = cellTileData;
        this.selectedCell = cell;
        
        upgradeResultTile.SetTile(autoUpgradeComponent.afterUpgradeTile);

        var factory = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        foreach (var upgradeStartRequirement in autoUpgradeComponent.upgradeStartRequirements)
        {
            var unlockRequirementUI = factory.CreateUIInstanceForRequirement(upgradeStartRequirement);
            unlockRequirementUI.Setup(upgradeStartRequirement, cell);
            upgradeRequirementsContainer.AddChild(unlockRequirementUI);
        }
    }
    
    public void Cleanup()
    {
        for (int i = upgradeRequirementsContainer.GetChildCount() - 1; i >= 0; i--)
        {
            upgradeRequirementsContainer.GetChild(i).QueueFree();
        }

        selectedCellTileData = null;
        selectedCell = Vector2I.MinValue;
    }
}
