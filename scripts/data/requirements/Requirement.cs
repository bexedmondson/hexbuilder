using Godot;

[GlobalClass][Tool]
public abstract partial class Requirement : Resource
{
    public abstract bool IsSatisfied();
}
