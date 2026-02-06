using System;
using Godot;

public partial class TimedJobCellInfoUI : Control
{
    [Export]
    private Label workerCountLabel;

    [Export]
    private Control hourglass;
    
    [Export]
    private Label turnCountLabel;

    [Export]
    private Container workerLabelContainer;

    [Export]
    private AnimationPlayer animationPlayer;

    public void UpdateWorkerCountLabel(int count, int capacity)
    {
        workerLabelContainer.Visible = capacity != 0;
        
        workerCountLabel.Text = $"{count}/{capacity}";
        hourglass.Visible = count >= 0;
    }

    public void UpdateTurnCountLabel(int turnCount)
    {
        turnCountLabel.Text = turnCount.ToString();
    }

    public void DoTurnAnim()
    {
        hourglass.SetPivotOffset(hourglass.Size / 2);
        animationPlayer.Play("turn_hourglass");
    }

    public void AnimateOut(string animationName, Action OnAnimationFinished)
    {
        //this is silly but such is godot's control pivot handling that i can't set it to just "50% of size" to
        //allow scaling to the center, instead i have to do this
        workerLabelContainer.SetPivotOffset(workerLabelContainer.Size / 2);
        animationPlayer.AnimationFinished += _ => OnAnimationFinished?.Invoke();
        animationPlayer.Play(animationName);
    }
}

