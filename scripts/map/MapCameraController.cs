using Godot;

public partial class MapCameraController : Camera2D, IInjectable
{
    [Export]
    private float defaultZoom = 1.0f;
    
    [Export]
    private float minZoom;
    
    [Export]
    private float maxZoom;

    [Export]
    private float scrollZoomIncrement = 0.3f;

    [Export]
    private Curve flyCurve;

    private Tween activeFlyTween;
    private Tween activeZoomTween;

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Register(this);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print("camera " + @event);
        if (@event is InputEventScreenDrag dragEvent)
        {
            Position -= dragEvent.ScreenRelative;
            this.GetViewport().SetInputAsHandled();
        }
        else if (@event is InputEventMagnifyGesture pinchGesture)
        {
            GD.Print(pinchGesture);
            HandleZoom(pinchGesture.Factor - 1);
            this.GetViewport().SetInputAsHandled();
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
        
        GD.Print($"[MapCameraController] HandleZoom: current zoomScale is {zoomScale}, factor is {factor}");
            
        if (factor < 0)
            zoomScale = Mathf.Max(zoomScale + factor, minZoom);
        else
            zoomScale = Mathf.Min(zoomScale + factor, maxZoom);

        Zoom = Vector2.One * zoomScale;
    }

    public void ResetZoom(float flyDuration = 0.5f)
    {
        if (Zoom == Vector2.One * defaultZoom)
            return;
        
        TweenZoomTo(Vector2.One * defaultZoom, flyDuration);
    }

    private void TweenZoomTo(Vector2 targetZoom, float duration = 0.5f)
    {
        if (activeZoomTween != null && activeZoomTween.IsRunning())
            activeZoomTween.Kill();
        
        activeZoomTween = CreateTween();
        activeZoomTween.SetEase(Tween.EaseType.InOut);
        activeZoomTween.TweenProperty(this, "zoom", targetZoom, duration);
        activeZoomTween.Play();
    }

    public void FlyToCellWithOffset(Vector2I cell, Vector2I screenOffset, float duration = 0.5f)
    {
        var target = InjectionManager.Get<MapController>().BaseMapLayer.GetCellCentreWorldPosition(cell);
        FlyToWorldPosition(target + screenOffset, duration);
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
