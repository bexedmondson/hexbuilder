using Godot;

public partial class MapCameraController : Camera2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenDrag dragEvent)
        {
            Position -= dragEvent.ScreenRelative;
        }
    }
}
