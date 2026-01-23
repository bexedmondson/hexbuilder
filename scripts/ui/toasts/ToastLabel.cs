using System;
using Godot;

[Tool]
public partial class ToastLabel : Panel
{
    [Export]
    private Label label;
    
    [Export]
    private int duration = 3; //seconds

    [ExportToolButton("anim")]
    private Callable test => Callable.From(() => Init("+10"));
    
    public Action<ToastLabel> RemoveLabelAction;

    // offset position box with screen position
    private Tween tweenIn;

    private float maskWidth;
    private float maskHeight;

    public void Init(string toastText)
    {
        label.Text = toastText;
        Callable.From(Animate).CallDeferred();
    }

    private void Animate()
    {
        float labelHeight = label.Size.Y;
        StyleBoxFlat labelStylebox = label.GetThemeStylebox("normal") as StyleBoxFlat;

        Vector2 initialLabelOffset = new Vector2(labelStylebox.ExpandMarginLeft, -labelHeight - labelStylebox.ExpandMarginBottom - labelStylebox.ShadowOffset.Y - labelStylebox.ShadowSize);
        Vector2 targetLabelOffset = new Vector2(labelStylebox.ExpandMarginLeft, labelStylebox.ExpandMarginTop);
        maskHeight = labelHeight + labelStylebox.ExpandMarginTop + labelStylebox.ExpandMarginBottom + labelStylebox.ShadowOffset.Y + labelStylebox.ShadowSize;
        maskWidth = label.Size.X + labelStylebox.ExpandMarginLeft + labelStylebox.ExpandMarginLeft;
        
        //GD.Print($"label width: {label.Size.X}, init label offset: {initialLabelOffset}, target label offset: {targetLabelOffset}, target min mask height: {targetMinimumMaskHeight}");
        
        tweenIn = GetTree().CreateTween();
        tweenIn.SetPauseMode(Tween.TweenPauseMode.Stop);
        tweenIn.Stop();
        tweenIn.SetParallel(true);
        tweenIn.TweenProperty(this, "custom_minimum_size", new Vector2(maskWidth, maskHeight), 0.3).From(new Vector2(maskWidth, 0)).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In).SetDelay(0.03);
        tweenIn.TweenProperty(label, "position", targetLabelOffset, 0.3).From(initialLabelOffset).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In).SetDelay(0.03);
        tweenIn.SetParallel(false);
        tweenIn.TweenProperty(this, "modulate:a", 0, 0.6).SetDelay(duration);
        tweenIn.TweenProperty(this, "custom_minimum_size", new Vector2(maskWidth, 0.0f), 0.2).From(new Vector2(maskWidth, maskHeight)).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
        tweenIn.TweenCallback(Callable.From(_TweenDestroyLabelComplete));
        tweenIn.Play();
    }

    private void _TweenDestroyLabelComplete()
    {
        RemoveLabelAction?.Invoke(this);
        QueueFree();
    }
}