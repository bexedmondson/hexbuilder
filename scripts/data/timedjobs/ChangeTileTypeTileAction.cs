using Godot;

[GlobalClass]
public partial class ChangeTileTypeTileAction : AbstractTileAction
{
    [Export]
    public CustomTileData tileToChangeTo { get; private set; }
}
