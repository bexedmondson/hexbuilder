using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class NeedConfig : Resource
{
    [Export]
    public Array<Requirement> assignmentRequirements;

    [Export]
    public Array<Requirement> satisfactionRequirements;

    [Export]
    public int unsatisfiedHappinessPenalty;
    
    [Export]
    public int satisfiedHappinessBonus;

    public bool CanAssignToResident(ResidentState resident)
    {
        foreach (var assignmentRequirement in assignmentRequirements)
        {
            if (assignmentRequirement is ResidentRequirement residentRequirement 
                && residentRequirement.IsSatisfied(resident))
                continue;
            else if (assignmentRequirement.IsSatisfied())
                continue;
            
            return false;
        }

        return true;
    }

    public bool IsSatisfied(ResidentState resident)
    {
        foreach (var satisfactionRequirement in satisfactionRequirements)
        {
            if (satisfactionRequirement is ResidentRequirement residentRequirement 
                && residentRequirement.IsSatisfied(resident))
                continue;
            else if (satisfactionRequirement.IsSatisfied())
                continue;
            
            return false;
        }

        return true;
    }
}
