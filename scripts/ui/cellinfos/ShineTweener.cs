using Godot;

[Tool]
public partial class ShineTweener : Control
{
    [Export]
    public Control itemWithShineShader;

    private Tween tween;

    public void SetShining(bool isOn)
    {
        if (isOn)
            StartTween();
        else if (tween != null)
            tween.Kill();
    }

    private void StartTween()
    {
        if (tween != null && tween.IsRunning())
            tween.Kill();

        tween = itemWithShineShader.CreateTween();
        tween.TweenProperty(itemWithShineShader.Material, "shader_parameter/shine_progress", 0.5f, 1.5f).From(0.0f);
        tween.SetLoops();
        tween.Play();
    }
}