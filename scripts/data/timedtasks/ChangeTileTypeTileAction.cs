using Godot;

[GlobalClass]
public partial class ChangeTileTypeTileAction : AbstractTileAction
{
    [Export]
    private CustomTileData tileToChangeTo;

    public virtual CustomTileData GetTileToChangeTo(Vector2I cell)
    {
        return tileToChangeTo;
    }

    public override void DoAction(Vector2I cell)
    {
        InjectionManager.Get<MapController>().BuildTileAtCell(cell, GetTileToChangeTo(cell));
    }
}
