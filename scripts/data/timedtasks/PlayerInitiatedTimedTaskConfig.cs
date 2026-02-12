using Godot;

[GlobalClass]
public partial class PlayerInitiatedTimedTaskConfig : TimedTaskConfig
{
    [Export]
    public int workersNeeded { get; private set; } = 1;
}
