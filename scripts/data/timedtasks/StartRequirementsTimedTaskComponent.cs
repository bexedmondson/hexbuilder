using Godot;

[GlobalClass]
public partial class StartRequirementsTimedTaskComponent : AbstractTimedTaskComponent
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> requirements { get; private set; } = new();
}
