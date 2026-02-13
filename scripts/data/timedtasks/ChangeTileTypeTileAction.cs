using Godot;

[GlobalClass]
public partial class ChangeTileTypeTileAction : AbstractTileAction
{
    [Export]
    public CustomTileData tileToChangeTo { get; private set; }

    public override void DoAction(Vector2I cell)
    {
        InjectionManager.Get<MapController>().BuildTileAtCell(cell, tileToChangeTo);
    }
}
