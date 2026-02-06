using Godot;

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

public abstract class CellInfoRequirementProcessor<TRequirement>(TRequirement dataRequirement) 
    : DataRequirementProcessor<Vector2I> where TRequirement : DataRequirement
{
    protected TRequirement dataRequirement = dataRequirement;
}

public abstract class WorkplaceStateRequirementProcessor<TRequirement>(TRequirement dataRequirement) 
    : DataRequirementProcessor<WorkplaceState> where TRequirement : DataRequirement
{
    protected TRequirement dataRequirement = dataRequirement;
}