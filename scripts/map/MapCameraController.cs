using Godot;

public partial class MapCameraController : Camera2D, IInjectable
{
    [Export]
    private float minZoom;
    
    [Export]
    private float maxZoom;

    [Export]
    private float scrollZoomIncrement = 0.3f;

    [Export]
    private Curve flyCurve;

    private Tween activeFlyTween;

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Register(this);
    }

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

    public void FlyToCell(Vector2I cell, float duration = 0.5f)
    {
        var target = InjectionManager.Get<MapController>().BaseMapLayer.GetCellCentreWorldPosition(cell);
        FlyToWorldPosition(target, duration);
    }

    public void FlyToWorldPosition(Vector2 worldPosition, float duration = 0.5f)
    {
        if (activeFlyTween != null && activeFlyTween.IsRunning())
            activeFlyTween.Kill();
        
        activeFlyTween = CreateTween();
        activeFlyTween.SetEase(Tween.EaseType.InOut);
        activeFlyTween.TweenProperty(this, "position", worldPosition, duration);
        activeFlyTween.Play();
    }
}
