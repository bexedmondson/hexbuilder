using System;
using Godot;

public partial class TimedJobCellInfoUI : Control
{
    [Export]
    private Label workerCountLabel;

    [Export]
    private Control alert;

    [Export]
    private Container workerLabelContainer;

    [Export]
    private AnimationPlayer animationPlayer;

    public void UpdateWorkerCountLabel(int count, int capacity)
    {
        workerCountLabel.Visible = capacity != 0;
        
        workerCountLabel.Text = $"{count}/{capacity}";
        alert.Visible = count <= 0 && capacity != 0;
    }

    public void AnimateOut(Action OnAnimationFinished)
    {
        //this is silly but such is godot's control pivot handling that i can't set it to just "50% of size" to
        //allow scaling to the center, instead i have to do this
        workerLabelContainer.SetPivotOffset(workerLabelContainer.Size / 2);
        animationPlayer.AnimationFinished += _ => OnAnimationFinished?.Invoke();
        animationPlayer.Play("job_complete");
    }
}

