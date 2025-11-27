using Godot;

public class TimedJobStartedEvent(Vector2I location) : IEvent
{
    public Vector2I location { get; private set; } = location;
}