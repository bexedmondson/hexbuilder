using Godot;

public class CellUnlockTimedJobData(Vector2I location, int turnDuration, int workerCountRequirement) 
    : TimedJobData(location, turnDuration, workerCountRequirement)
{
    public override string description => "unlocking area";

    public override void CompleteJob()
    {
        InjectionManager.Get<MapController>().UnlockCell(location);
    }
}
