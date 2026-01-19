using System;
using Godot;
using Godot.Collections;

public partial class ToastLabel : Label
{
    public Action<ToastLabel> RemoveLabelAction;

    // Text margin with box parent
    private Dictionary<string, float> margins = new(){
        {
            "left", 12
        },{
            "top", 7
        },{
            "right", 12
        },{
            "bottom", 7
        },
    };

    // margin between buttons
    private const int MarginBetween = 23;
    private Vector2 resolution = Vector2.Zero;

    // offset position box with screen position
    private Vector2 offsetPosition = new Vector2(10, 10);
    private Vector2 buttonSize;
    private Tween tweenIn;
    public ToastDirection direction = ToastDirection.TopCenter;
    private int timerToDestroy = 3;

    // seconds by default
    public override void _Ready()
    {
        SetResolution();
        buttonSize = this.Size;

        // start position
        _TweenDestroyLabelTimer();
    }

    public void Init(ToastConfig config)
    {
        direction = config.direction;
        this.Position = new Vector2(Position.X, GetYPos(-100, config.direction));
        UpdateText(config.text);
    }

    private void UpdateText(string text)
    {
        this.Text = text;
        buttonSize = this.Size;
        UpdateXPosition();
    }

    public void MoveTo(int index)
    {
        UpdateXPosition();
        float offsetY = (MarginBetween + buttonSize.Y) * index;
        float y = GetYPos(offsetY, direction);

        // bottom
        if (index == 0)
        {
            tweenIn = GetTree().CreateTween();
            tweenIn.SetPauseMode(Tween.TweenPauseMode.Stop);
            // pause mode
            tweenIn.Stop();
            tweenIn.TweenProperty(this, "position", new Vector2(Position.X, y), 0.3).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In).SetDelay(0.03);
            tweenIn.Play();
        }
        else
        {
            tweenIn = GetTree().CreateTween();
            tweenIn.SetPauseMode(Tween.TweenPauseMode.Stop);
            // pause mode
            tweenIn.Stop();
            tweenIn.TweenProperty(this, "position", new Vector2(Position.X, y), 0.3).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.InOut);
            tweenIn.Play();
        }
    }

    private void _TweenDestroyLabelComplete()
    {
        RemoveLabelAction?.Invoke(this);
        QueueFree();
    }

    private void _TweenDestroyLabelTimer()
    {
        // tween alpha to 0
        var tweenAlpha = GetTree().CreateTween();
        tweenAlpha.SetPauseMode(Tween.TweenPauseMode.Stop);
        // pause mode
        tweenAlpha.TweenProperty(this, "modulate:a", 0, 0.8).SetDelay(timerToDestroy);
        tweenAlpha.TweenCallback(Callable.From(_TweenDestroyLabelComplete));
    }

    private float GetYPos(float offset, ToastDirection direction)
    {
        // left position.x = margins.left + offset_position.x
        float yPos = 0;
        if (direction == ToastDirection.TopLeft || direction == ToastDirection.TopCenter || direction == ToastDirection.TopRight)
        {
            yPos = margins["top"] + offsetPosition.Y + offset;
        }
        else
        {
            yPos = resolution.Y - margins["top"] - buttonSize.Y - offsetPosition.Y - offset;
        }
        return yPos;
    }

    public void UpdateXPosition()
    {
        SetResolution();
        if (direction == ToastDirection.TopLeft || direction == ToastDirection.BottomLeft)
        {
            Position = new Vector2(margins["left"] + offsetPosition.X, Position.Y);
        }
        else if (direction == ToastDirection.TopCenter || direction == ToastDirection.BottomCenter)
        {
            Position = new Vector2((resolution.X / 2) - (Size.X / 2), Position.Y);
        }
        else
        {
            Position = new Vector2(resolution.X - margins["left"] - Size.X - offsetPosition.X, Position.Y);
        }
    }

    private void SetResolution()
    {
        resolution.X = GetViewport().GetVisibleRect().Size.X;
        resolution.Y = GetViewport().GetVisibleRect().Size.Y;
    }
}