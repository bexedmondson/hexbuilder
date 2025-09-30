
using Godot;
using Godot.Collections;

public partial class BaseTileMapLayer : TileMapLayer
{
    //this is a workaround for issue https://github.com/godotengine/godot/issues/107342
    public override void _UpdateCells(Array<Vector2I> coords, bool forcedCleanup)
    {
        EmitSignalChanged();
    }
}
