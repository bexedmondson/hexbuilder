using Godot;

public class TimedJobStartedEvent(TimedJobState timedJob, Vector2I location, int workerCount, int capacity) : IEvent
{
    public TimedJobState timedJob { get; private set; } = timedJob;
    public Vector2I location { get; private set; } = location;

    public int workerCount { get; private set; } = workerCount;
    public int capacity { get; private set; } = capacity;
}