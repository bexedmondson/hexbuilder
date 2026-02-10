using Godot;

[GlobalClass][Tool]
public partial class ResidentCapacityComponent : AbstractTileDataComponent
{
    [Export]
    public int capacity { get; private set; }
}
