using Godot;

[Tool]
public abstract partial class Requirement<T> : AbstractRequirement where T : class
{
    public abstract bool IsSatisfied(T data);
}