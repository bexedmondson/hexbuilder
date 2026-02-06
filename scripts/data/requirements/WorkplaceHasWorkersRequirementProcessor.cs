public class WorkplaceHasWorkersRequirementProcessor(WorkplaceHasWorkersRequirement dataRequirement) : WorkplaceStateRequirementProcessor<WorkplaceHasWorkersRequirement>(dataRequirement)
{
    public override bool IsSatisfied(WorkplaceState workplace)
    {
        return workplace.workerCount > 0;
    }
}
