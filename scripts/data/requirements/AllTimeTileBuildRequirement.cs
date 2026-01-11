using Godot;

[GlobalClass][Tool]
public partial class AllTimeTileBuildRequirement : Requirement
{
    [Export]
    public Godot.Collections.Array<TileRequirementAmount> requiredTileBuildCounts = new();

    public override bool IsSatisfied()
    {
        var statsTracker = InjectionManager.Get<CellBuildStatsTracker>();
        
        if (requiredTileBuildCounts != null)
        {
            foreach (var tileReqCount in requiredTileBuildCounts)
            {
                if (!statsTracker.GetBuildCount(tileReqCount.tile).IsPass(tileReqCount.comparison, tileReqCount.amount))
                    return false;
            }
        }

        return true;
    }
}
