using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class NeedConfig : Resource
{
    [Export]
    public Array<AbstractRequirement> assignmentRequirements { get; private set; } = new();

    [Export]
    public Array<AbstractRequirement> satisfactionRequirements { get; private set; } = new();

    [Export]
    public int unsatisfiedHappinessPenalty { get; private set; }
    
    [Export]
    public int satisfiedHappinessBonus { get; private set; }
    
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
