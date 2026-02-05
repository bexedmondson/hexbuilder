using System;
using Godot;

public class AutoUpgradeTimedJobState(Vector2I location, int turnDuration, int workerCountRequirement) : TimedJobState(location, turnDuration, workerCountRequirement)
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
