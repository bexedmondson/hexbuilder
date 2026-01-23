using Godot;

[GlobalClass][Tool]
public partial class LifetimeGainedRequirement : Requirement
{
    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> requiredAmountsGained;

    [Export]
    public CurrencyType currency;

    [Export]
    public int requiredAmount;

    public override bool IsSatisfied()
    {
        var statsTracker = InjectionManager.Get<InventoryStatsTracker>();
        return statsTracker.GetGainedAmount(currency) >= requiredAmount;
    }
}
