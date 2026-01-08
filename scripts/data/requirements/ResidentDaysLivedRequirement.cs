using Godot;

[GlobalClass][Tool]
public partial class ResidentDaysLivedRequirement : DataRequirement
{
    [Export]
    public int minimumDaysLived;

    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new ResidentDaysLivedRequirementProcessor(this);
    }
}
