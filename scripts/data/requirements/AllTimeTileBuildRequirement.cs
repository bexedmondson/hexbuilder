using Godot;

[GlobalClass][Tool]
public partial class AllTimeTileBuildRequirement : Requirement
{
    [Export]
    public TileRequirementAmount requiredTileBuildCount = new();

    public override bool IsSatisfied()
    {
        var buildCount = InjectionManager.Get<CellBuildStatsTracker>().GetBuildCount(requiredTileBuildCount.tile);

        return requiredTileBuildCount.IsPass(buildCount);
    }
}
