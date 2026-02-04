using Godot;

[GlobalClass][Tool]
public partial class AutoUpgradeComponent : AbstractTileDataComponent, IRequirementContainer
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> upgradeStartRequirements = new();

    [Export]
    public int upgradeDuration; //TODO implement

    [Export]
    public CustomTileData afterUpgradeTile;

    public bool CanStartUpgrade(WorkplaceState workplaceState = null)
    {
        return (this as IRequirementContainer).GetAreRequirementsSatisfied(upgradeStartRequirements, workplaceState);
    }
}
