using Godot;

public class TimedTaskStartedEvent(TimedTaskState timedTask) : IEvent
{
    public TimedTaskState timedTask { get; } = timedTask;
    public Vector2I location => timedTask.location;

    public int workerCount => timedTask.workerCount;
    public int capacity => timedTask.workerCountRequirement;
}