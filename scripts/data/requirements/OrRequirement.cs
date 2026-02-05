using Godot;

[GlobalClass][Tool]
public partial class OrRequirement : DataRequirement
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> requirements = new();
    
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new OrRequirementProcessor(this);
    }
}
