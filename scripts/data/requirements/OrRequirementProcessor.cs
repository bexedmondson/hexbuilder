using Godot;

public class OrRequirementProcessor(OrRequirement dataRequirement) : DataRequirementProcessor<object>, IRequirementContainer
{
    public override bool IsSatisfied(object data)
    {
        foreach (var orRequirement in dataRequirement.requirements)
        {
            switch (orRequirement)
            {
                case DataRequirement dRequirement:
                    var processor = dRequirement.GetDataRequirementProcessor();
                    if ((this as IRequirementContainer).GetDataRequirementSatisfaction(processor, data))
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
}
