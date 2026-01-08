using Godot;

[GlobalClass][Tool]
public abstract partial class DataRequirement : AbstractRequirement
{
    public abstract DataRequirementProcessor GetDataRequirementProcessor();
}