public abstract class DataRequirementProcessor
{
}

public abstract class DataRequirementProcessor<TData> : DataRequirementProcessor
{
    public abstract bool IsSatisfied(TData data);
}

public abstract class ResidentStateRequirementProcessor<TRequirement>(TRequirement dataRequirement) 
    : DataRequirementProcessor<ResidentState> where TRequirement : DataRequirement
{
    protected TRequirement dataRequirement = dataRequirement;
}