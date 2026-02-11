using Godot;

[GlobalClass]
public partial class CancelRequirementsTimedJobComponent : AbstractTimedJobComponent
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> requirements { get; private set; } = new();
}