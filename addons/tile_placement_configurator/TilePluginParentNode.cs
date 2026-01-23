using Godot;

[Tool]
public partial class TilePluginParentNode : Control
{
    [Export]
    private TileSet tileSet;

    public override void _EnterTree()
    {
        EditorTileDatabase.AddTileSetTileData(tileSet);
        //GD.Print("enter");
    }

    public override void _Ready()
    {
        base._Ready();
        EditorTileDatabase.AddTileSetTileData(tileSet);
    }

    public void AddTileData()
    {
        EditorTileDatabase.AddTileSetTileData(tileSet);
    }

    public override void _ExitTree()
    {
        base._ExitTree(); 
        //GD.Print("exit");
    }
}
 