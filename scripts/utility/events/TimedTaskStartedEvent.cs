using Godot;

public class TimedTaskStartedEvent(TimedTaskState timedTask, Vector2I location, int workerCount, int capacity) : IEvent
{
    public TimedTaskState timedTask { get; private set; } = timedTask;
    public Vector2I location { get; private set; } = location;

    public int workerCount { get; private set; } = workerCount;
    public int capacity { get; private set; } = capacity;
}