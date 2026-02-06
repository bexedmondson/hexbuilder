public partial class WorkplaceHasWorkersRequirement : DataRequirement
{
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new WorkplaceHasWorkersRequirementProcessor(this);
    }
}
