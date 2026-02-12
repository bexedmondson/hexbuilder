using Godot;

[GlobalClass][Tool]
public partial class TileTimedTasksComponent : AbstractTileDataComponent
{
    [Export]
    public Godot.Collections.Array<TimedTaskConfig> tileTimedTasks { get; private set; } = new();
}
