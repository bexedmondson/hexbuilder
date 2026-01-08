using Godot;

[GlobalClass][Tool]
public abstract partial class ResidentRequirement : Requirement
{
    public abstract bool IsSatisfied(ResidentState resident);
}
