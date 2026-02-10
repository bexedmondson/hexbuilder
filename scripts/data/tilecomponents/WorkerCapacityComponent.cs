using Godot;

[GlobalClass][Tool]
public partial class WorkerCapacityComponent : AbstractTileDataComponent
{
    [Export]
    public int capacity { get; private set; }
}
