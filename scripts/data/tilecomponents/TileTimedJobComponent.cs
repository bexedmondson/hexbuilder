using Godot;

[GlobalClass][Tool]
public partial class TileTimedJobComponent : AbstractTileDataComponent
{
    [Export]
    public Godot.Collections.Array<TimedJobConfig> tileTimedJobs { get; private set; } = new();
}
