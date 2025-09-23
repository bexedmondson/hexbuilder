using Godot;

public partial class MapCameraController : Camera2D
{
    [Export]
    private float minZoom;
    
    [Export]
    private float maxZoom;

    [Export]
    private float scrollZoomIncrement = 0.3f;
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenDrag dragEvent)
        {
            Position -= dragEvent.ScreenRelative;
        }
        else if (@event is InputEventMagnifyGesture pinchGesture)
        {
            HandleZoom(pinchGesture.Factor);
        }
        else if (@event is InputEventMouseButton mouseWheelEvent && mouseWheelEvent.Pressed)
        {
            if (mouseWheelEvent.ButtonIndex == MouseButton.WheelDown)
                HandleZoom(-scrollZoomIncrement);
            else if (mouseWheelEvent.ButtonIndex == MouseButton.WheelUp)
                HandleZoom(scrollZoomIncrement);
        }
    }

    private void HandleZoom(float factor)
    {
        var zoomScale = Zoom.X; //both dimensions should be the same here
            
        if (factor < 0)
            zoomScale = Mathf.Max(zoomScale + factor, minZoom);
        else
            zoomScale = Mathf.Min(zoomScale + factor, maxZoom);

        Zoom = Vector2.One * zoomScale;
    }
}
