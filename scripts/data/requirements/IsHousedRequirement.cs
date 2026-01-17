using Godot;

[GlobalClass][Tool]
public partial class IsHousedRequirement : DataRequirement
{
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new IsHousedRequirementProcessor(this);
    }
}
