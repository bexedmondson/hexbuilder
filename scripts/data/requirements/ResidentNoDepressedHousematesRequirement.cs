using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class ResidentNoDepressedHousematesRequirement : DataRequirement
{
    [Export]
    public int minimumAcceptableHappiness;
    
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new ResidentNoDepressedHousematesRequirementProcessor(this);
    }
}
