using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class NeedConfig : Resource
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

        return RequirementCalculation.GetAreRequirementsSatisfied(assignmentRequirements, resident);
    }

    public bool IsNeedSatisfied<T>(T data) where T : new()
    {
        return RequirementCalculation.GetAreRequirementsSatisfied(satisfactionRequirements, data);
    }
}
