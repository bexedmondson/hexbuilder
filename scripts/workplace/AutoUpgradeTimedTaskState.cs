using System;
using Godot;

public class AutoUpgradeTimedTaskState(Vector2I location, int turnDuration, int workerCountRequirement) : TimedTaskState(location, turnDuration, workerCountRequirement)
{
    public override string description => "upgrading...";

    private Action completeAction;

    public void SetCompleteCallback(Action completeAction)
    {
        this.completeAction = completeAction;
    }

    public override void CompleteJob()
    {
        completeAction?.Invoke();
    }
}
