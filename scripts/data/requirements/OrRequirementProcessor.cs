using Godot;

public class OrRequirementProcessor(OrRequirement dataRequirement) : DataRequirementProcessor<object>
{
    public override bool IsSatisfied(object data)
    {
        foreach (var orRequirement in dataRequirement.requirements)
        {
            switch (orRequirement)
            {
                case DataRequirement dRequirement:
                    var processor = dRequirement.GetDataRequirementProcessor();
                    if (GetDataRequirementSatisfaction(processor, data))
                        return true;
                    break;
                case Requirement requirement:
                    if (requirement.IsSatisfied())
                        return true;
                    break;
            }
        }
        
        return false;
    }
    
    private bool GetDataRequirementSatisfaction<T>(DataRequirementProcessor dataRequirementProcessor, T data) where T : class
    {
        if (data == null)
        {
            GD.PushWarning("OrRequirement.GetDataRequirementSatisfaction: trying to use data that's null! Is this intentional?");
            return false;
        }
        
        if (data is ResidentState resident && dataRequirementProcessor is DataRequirementProcessor<ResidentState> residentRequirementProcessor)
            return residentRequirementProcessor.IsSatisfied(resident);
        else if (data is WorkplaceState workplace && dataRequirementProcessor is DataRequirementProcessor<WorkplaceState> workplaceRequirementProcessor)
            return workplaceRequirementProcessor.IsSatisfied(workplace);
        
        GD.PushError("OrRequirement.GetDataRequirementSatisfaction: found data requirement that isn't handled!");
        return false;
    }
}
