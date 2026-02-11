using Godot;

public partial class PlayerInitiatedTimedJobConfig : TimedJobConfig
{
    [Export]
    public int workersNeeded { get; private set; } = 1;
}
