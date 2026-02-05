using Godot;

public partial class UnlockedCellPopup_AutoUpgrade : Control
{
    [Export]
    private TileTextureRect upgradeResultTile;

    [Export]
    private Control upgradeRequirementsContainer;

    [Export]
    private Control upgradeRequirementsParent;

    [Export]
    private PackedScene upgradeRequirementScene;

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
        
        foreach (var upgradeStartRequirement in autoUpgradeComponent.upgradeStartRequirements)
        {
            var unlockRequirementUI = upgradeRequirementScene.Instantiate<UnlockRequirementUI>();
            unlockRequirementUI.Setup(upgradeStartRequirement, cell);
            upgradeRequirementsContainer.AddChild(unlockRequirementUI);
        }
    }
    
    private void SetupUnlockRequirements(CustomTileData tileData, Vector2I cell)
    {
        for (int i = upgradeRequirementsContainer.GetChildCount() - 1; i >= 0; i--)
        {
            upgradeRequirementsContainer.GetChild(i).QueueFree();
        }

        if (tileData.IsUnlocked() || !tileData.TryGetComponent(out UnlockRequirementsComponent unlockRequirementsComponent))
        {
            upgradeRequirementsParent.Visible = false;
            return;
        }

        upgradeRequirementsParent.Visible = true;
        foreach (var requirement in unlockRequirementsComponent.requirements)
        {
            var requirementUI = upgradeRequirementScene.Instantiate<UnlockRequirementUI>();
            requirementUI.Setup(requirement, cell);
            upgradeRequirementsContainer.AddChild(requirementUI);
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
