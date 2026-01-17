using Godot;

[GlobalClass][Tool]
public partial class AllTimeTileBuildRequirement : Requirement
{
    [Export]
    public Godot.Collections.Array<TileRequirementAmount> requiredTileBuildCounts;
    [Export]
    public TileRequirementAmount requiredTileBuildCount;
    [ExportToolButton("update")]
    public Callable ClickMeButton => Callable.From(Update);

    public override bool IsSatisfied()
    {
        var statsTracker = InjectionManager.Get<CellBuildStatsTracker>();
        
        if (requiredTileBuildCount != null)
        {
            if (!statsTracker.GetBuildCount(requiredTileBuildCount.tile).IsPass(requiredTileBuildCount.comparison, requiredTileBuildCount.amount))
                return false;
        }

        return true;
    }

    public void Update()
    {
        requiredTileBuildCount = requiredTileBuildCounts[0] as TileRequirementAmount;
        NotifyPropertyListChanged();
    }
}
