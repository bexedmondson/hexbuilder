using Godot;

[Tool]
public class ToastConfig
{
    public string text = "Toast Label";
    public string stackId = string.Empty;
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