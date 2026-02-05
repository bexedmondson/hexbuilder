using Godot;

[GlobalClass][Tool]
public partial class AutoUpgradeComponent : AbstractTileDataComponent, IRequirementContainer
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> upgradeStartRequirements = new();

    [Export]
    public int upgradeDuration;

    [Export]
    public CustomTileData afterUpgradeTile;

    public bool CanStartUpgrade(Vector2I cell)
    {
        return (this as IRequirementContainer).GetAreRequirementsSatisfied(upgradeStartRequirements, cell);
    }
}
