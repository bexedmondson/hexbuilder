using Godot;

[GlobalClass]
public partial class CancelRequirementsTimedTaskComponent : AbstractTimedTaskComponent
{
    [Export]
    public Godot.Collections.Array<AbstractRequirement> requirements { get; private set; } = new();
}