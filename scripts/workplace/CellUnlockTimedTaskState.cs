using Godot;

public class CellUnlockTimedTaskState(Vector2I location, int turnDuration, int workerCountRequirement) 
    : TimedTaskState(location, turnDuration, workerCountRequirement)
{
    public override string description => "unlocking area";

    public override void CompleteJob()
    {
        InjectionManager.Get<MapController>().UnlockCell(location);
    }
}
