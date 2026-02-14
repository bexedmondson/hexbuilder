using Godot;

public partial class ChangeTileTimedTaskCompleteActionUI : AbstractTimedTaskCompleteActionUI
{
    [Export]
    private TileTextureRect tileToChangeTo;

    public override void Setup(AbstractTileAction completeAction, Vector2I cell)
    {
        var action = completeAction as ChangeTileTypeTileAction;
        tileToChangeTo.SetTile(action.GetTileToChangeTo(cell));
    }
}
