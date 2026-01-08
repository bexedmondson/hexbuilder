using Godot;

[GlobalClass][Tool]
public abstract partial class Requirement : AbstractRequirement
{
    public abstract bool IsSatisfied();
}