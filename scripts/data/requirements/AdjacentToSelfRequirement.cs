using Godot;

[GlobalClass][Tool]
public partial class AdjacentToSelfRequirement : DataRequirement
{
    //this whole class is a workaround for the fact that godot's resources can't reference themselves,
    //otherwise I'd just use AdjacencyRequirement
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new AdjacentToSelfRequirementProcessor(this);
    }
}