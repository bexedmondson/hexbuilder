public class IsHousedRequirementProcessor(IsHousedRequirement dataRequirement) : ResidentStateRequirementProcessor<IsHousedRequirement>(dataRequirement)
{
    public override bool IsSatisfied(ResidentState resident)
    {
        return resident.HasHouse;
    }
}
