using Godot;

public class ToastConfig
{
    public string text = "Toast Label";
    public ToastDirection direction = ToastDirection.TopLeft;
}

public enum ToastDirection
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}