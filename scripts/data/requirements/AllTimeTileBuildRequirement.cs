using Godot;

[GlobalClass][Tool]
public partial class AllTimeTileBuildRequirement : Requirement
{
    [Export]
    public TileRequirementAmount requiredTileBuildCount;

    public override bool IsSatisfied()
    {
        var buildCount = InjectionManager.Get<CellBuildStatsTracker>().GetBuildCount(requiredTileBuildCount.tile);

        return buildCount.IsPass(requiredTileBuildCount.comparison, requiredTileBuildCount.amount);
    }
}
