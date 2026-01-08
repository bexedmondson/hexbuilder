public class ResidentDaysLivedRequirementProcessor : ResidentStateRequirementProcessor<ResidentDaysLivedRequirement>
{
    public ResidentDaysLivedRequirementProcessor(ResidentDaysLivedRequirement residentDaysLivedRequirement) : base(residentDaysLivedRequirement)
    {
    }
    
    public override bool IsSatisfied(ResidentState resident)
    {
        int currentTurnCount = InjectionManager.Get<TurnCounter>().turnCount;
        return currentTurnCount - resident.moveInDay > dataRequirement.minimumDaysLived;
    }
}
