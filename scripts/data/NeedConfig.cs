using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class NeedConfig : Resource, IRequirementContainer
{
    [Export]
    public Array<AbstractRequirement> assignmentRequirements;

    [Export]
    public Array<AbstractRequirement> satisfactionRequirements;

    [Export]
    public int unsatisfiedHappinessPenalty;
    
    [Export]
    public int satisfiedHappinessBonus;
    
    public bool CanAssignToResident(ResidentState resident)
    {
        if (resident.activeNeeds.Contains(this))
            return false;

        return (this as IRequirementContainer).GetAreRequirementsSatisfied(assignmentRequirements, resident);
    }

    public bool IsNeedSatisfied<T>(T data) where T : new()
    {
        return (this as IRequirementContainer).GetAreRequirementsSatisfied(satisfactionRequirements, data);
    }
}
