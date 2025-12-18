using Godot;

[GlobalClass][Tool]
public partial class AllTimeTileBuildUnlockRequirement : UnlockRequirement
{
    [Export]
    public Godot.Collections.Dictionary<CustomTileData, int> requiredTilesBuildCounts = new();

    public override bool IsSatisfied()
    {
        var statsTracker = InjectionManager.Get<CellBuildStatsTracker>();
        
        foreach (var kvp in requiredTilesBuildCounts)
        {
            if (statsTracker.GetBuildCount(kvp.Key) < kvp.Value)
                return false;
        }

        return true;
    }
}
