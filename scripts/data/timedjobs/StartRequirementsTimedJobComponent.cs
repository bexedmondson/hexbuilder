using Godot;

[GlobalClass]
public partial class StartRequirementsTimedJobComponent : AbstractTimedJobComponent
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> requirements { get; private set; } = new();
}
