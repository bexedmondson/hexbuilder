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
        
        foreach (var assignmentRequirement in assignmentRequirements)
        {
            switch (assignmentRequirement)
            {
                case DataRequirement dataRequirement:
                    var processor = dataRequirement.GetDataRequirementProcessor();
                    if (GetDataRequirementSatisfaction(processor, resident))
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
                case DataRequirement dataRequirement:
                    var processor = dataRequirement.GetDataRequirementProcessor();
                    if (GetDataRequirementSatisfaction(processor, resident))
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

    private bool GetDataRequirementSatisfaction(DataRequirementProcessor dataRequirementProcessor, ResidentState resident)
    {
        if (dataRequirementProcessor is DataRequirementProcessor<ResidentState> residentRequirementProcessor)
            return residentRequirementProcessor.IsSatisfied(resident);
        
        GD.PushError("NeedConfig IsSatisfied: found data requirement that isn't handled!");
        return false;
    }
}
