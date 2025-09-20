using Godot;

[Tool]
public partial class TilePluginParentNode : Control
{
    [Export]
    private TileSet tileSet;

    public override void _EnterTree()
    {
        InjectionManager.Get<TileDatabase>().AddTileSetTileData(tileSet);
    }
}
