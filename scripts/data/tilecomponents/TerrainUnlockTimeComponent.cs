using Godot;

[GlobalClass][Tool]
public partial class TerrainUnlockTimeComponent : AbstractTileDataComponent
{
    [Export]
    public int turnCount = 1;
}
