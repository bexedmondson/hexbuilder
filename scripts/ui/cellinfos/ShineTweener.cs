using Godot;

[Tool]
public partial class ShineTweener : Control
{
    [Export]
    public Control itemWithShineShader;

    [Export]
    private float finalShaderParamValue;
    
    [Export]
    private float finalShaderDurationValue;
    
    [Export]
    private bool shineByDefault = false;

    private Tween tween;

    public override void _Ready()
    {
        base._Ready();
        if (shineByDefault)
            SetShining(true);
    }

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
        tween.TweenProperty(itemWithShineShader.Material, "shader_parameter/shine_progress", finalShaderParamValue, finalShaderDurationValue).From(0.0f);
        tween.SetLoops();
        tween.Play();
    }
}