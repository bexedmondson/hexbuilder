using Godot;

public class TimedJobEndedEvent(Vector2I location) : IEvent
{
    public Vector2I location { get; private set; } = location;
}
