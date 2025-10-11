using Godot;

[GlobalClass][Tool]
public abstract partial class UnlockRequirement : Resource
{
    public abstract bool IsSatisfied();
}
