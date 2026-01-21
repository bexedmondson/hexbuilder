using System;
using Godot;
using Godot.Collections;

public partial class ToastLabel : Label
{
    public Action<ToastLabel> RemoveLabelAction;

    // offset position box with screen position
    private Vector2 buttonSize;
    private Vector2 buttonPosition;
    private Tween tweenIn;
    private int timerToDestroy = 3;

    // seconds by default
    public override void _Ready()
    {
        buttonPosition = this.Position;
        buttonSize = this.Size;

        // start position
        _TweenDestroyLabelTimer();
    }

    public void Init(ToastConfig config)
    {
        this.Text = config.text;
        buttonSize = this.Size;
        buttonPosition = this.Position;
    }

    public void MoveTo(int index)
    {
        tweenIn = GetTree().CreateTween();
        tweenIn.SetPauseMode(Tween.TweenPauseMode.Stop);
        tweenIn.Stop();
        tweenIn.TweenProperty(this, "position", buttonPosition, 0.3).From(new Vector2(buttonPosition.X, buttonPosition.Y - buttonSize.Y)).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In).SetDelay(0.03);
        tweenIn.Play();
    }

    private void _TweenDestroyLabelComplete()
    {
        RemoveLabelAction?.Invoke(this);
        QueueFree();
    }

    private void _TweenDestroyLabelTimer()
    {
        // tween alpha to 0 and shrink size
        var tweenOut = GetTree().CreateTween();
        tweenOut.SetPauseMode(Tween.TweenPauseMode.Stop);
        // pause mode
        tweenOut.TweenProperty(this, "modulate:a", 0, 0.8).SetDelay(timerToDestroy);
        tweenOut.TweenProperty(this, "position", new Vector2(buttonPosition.X, buttonPosition.Y - buttonSize.Y), 0.5).From(buttonPosition).SetDelay(timerToDestroy + 0.3);
        tweenOut.TweenCallback(Callable.From(_TweenDestroyLabelComplete));
    }
}