using Godot;

[GlobalClass][Tool]
public partial class LifetimeGainedUnlockRequirement : UnlockRequirement
{
    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> requiredAmountsGained;

    public override bool IsSatisfied()
    {
        var statsTracker = InjectionManager.Get<InventoryStatsTracker>();
        
        foreach (var kvp in requiredAmountsGained)
        {
            if (statsTracker.GetGainedAmount(kvp.Key) < kvp.Value)
                return false;
        }

        return true;
    }
}
