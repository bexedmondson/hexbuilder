using System.Collections.Generic;
using Godot;

public interface IRequirementContainer
{
    public bool GetAreRequirementsSatisfied<T>(IEnumerable<AbstractRequirement> requirements, T data) where T : new()
    {
        foreach (var satisfactionRequirement in requirements)
        {
            switch (satisfactionRequirement)
            {
                case DataRequirement dataRequirement:
                    var processor = dataRequirement.GetDataRequirementProcessor();
                    if (GetDataRequirementSatisfaction(processor, data))
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

    private bool GetDataRequirementSatisfaction<T>(DataRequirementProcessor dataRequirementProcessor, T data) where T : new()
    {
        if (data == null)
        {
            GD.PushWarning("GetDataRequirementSatisfaction: trying to use data that's null! Is this intentional?");
            return false;
        }
        
        if (data is ResidentState resident && dataRequirementProcessor is DataRequirementProcessor<ResidentState> residentRequirementProcessor)
            return residentRequirementProcessor.IsSatisfied(resident);
        else if (data is WorkplaceState workplace && dataRequirementProcessor is DataRequirementProcessor<WorkplaceState> workplaceRequirementProcessor)
            return workplaceRequirementProcessor.IsSatisfied(workplace);
        
        GD.PushError("GetDataRequirementSatisfaction: found data requirement that isn't handled!");
        return false;
    }
}
