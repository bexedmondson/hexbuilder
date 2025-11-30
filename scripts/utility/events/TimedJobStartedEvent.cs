using Godot;

public class TimedJobStartedEvent(Vector2I location, int workerCount, int capacity) : IEvent
{
    public Vector2I location { get; private set; } = location;

    public int workerCount { get; private set; } = workerCount;
    public int capacity { get; private set; } = capacity;
}