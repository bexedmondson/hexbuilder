using Godot;

public class ToastConfig
{
    public string text = "Toast Label";
    public Color bgColor = new Color(0,0,0, 0.7f);
    public Color color = new Color(1,1,1);
    public ToastDirection direction = ToastDirection.TopLeft;
    public int textSize = 18;
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