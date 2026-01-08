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
        foreach (var assignmentRequirement in assignmentRequirements)
        {
            switch (assignmentRequirement)
            {
                case Requirement<ResidentState> residentRequirement:
                    if (residentRequirement.IsSatisfied(resident))
                        continue;
                    break;
                case Requirement requirement:
                    if (requirement.IsSatisfied())
                        continue;
                    break;
            }
            
            return false;
        }

        return true;
    }

    public bool IsSatisfied(ResidentState resident)
    {
        foreach (var satisfactionRequirement in satisfactionRequirements)
        {
            switch (satisfactionRequirement)
            {
                case Requirement<ResidentState> residentRequirement:
                    if (residentRequirement.IsSatisfied(resident))
                        continue;
                    break;
                case Requirement requirement:
                    if (requirement.IsSatisfied())
                        continue;
                    break;
            }
            
            return false;
        }

        return true;
    }
}
