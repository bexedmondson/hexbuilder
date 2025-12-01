using Godot;

public class CellUnlockTimedJobState(Vector2I location, int turnDuration, int workerCountRequirement) 
    : TimedJobState(location, turnDuration, workerCountRequirement)
{
    public override string description => "unlocking area";

    public override void CompleteJob()
    {
        InjectionManager.Get<MapController>().UnlockCell(location);
    }
}
