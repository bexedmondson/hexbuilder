using Godot;

[GlobalClass][Tool]
public partial class AutoUpgradeComponent : AbstractTileDataComponent
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> upgradeStartRequirements  { get; private set; } = new();

    [Export]
    public int upgradeDuration { get; private set; }

    [Export]
    public CustomTileData afterUpgradeTile { get; private set; }

    public bool CanStartUpgrade(Vector2I cell)
    {
        bool canStart = RequirementCalculation.GetAreRequirementsSatisfied(upgradeStartRequirements, cell);
        if (canStart)
            GD.Print($"AutoUpgradeComponent: Starting upgrade at {cell} to {afterUpgradeTile.GetFileName()}");
        return canStart;
    }
}
