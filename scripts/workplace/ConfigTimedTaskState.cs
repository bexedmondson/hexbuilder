using System;
using Godot;

public class ConfigTimedTaskState(Vector2I location, TimedTaskConfig config) : TimedTaskState(location, config.turnDuration)
{
    private TimedTaskConfig config = config;
    
    public override string description => "upgrading...";

    public override int workerCountRequirement => config is PlayerInitiatedTimedTaskConfig pConfig ? pConfig.workersNeeded : 0;
    
    public override void CompleteJob()
    {
        foreach (var completeAction in config.completeActions)
        {
            completeAction.DoAction(location);
        }
    }
}
