using Godot;

[GlobalClass]
public partial class TimedJobConfig : Resource
{
    [Export]
    public string jobName { get; protected set; }

    [Export]
    public Texture2D icon { get; protected set; }

    [Export(PropertyHint.Range, "0,100,")]
    public int turnDuration { get; protected set; }

    [Export]
    public Godot.Collections.Array<AbstractTileAction> completeActions { get; protected set; } = new();

    [Export]
    public Godot.Collections.Array<AbstractTimedJobComponent> components { get; protected set; } = new();
}
