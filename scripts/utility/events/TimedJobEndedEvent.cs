using Godot;

public class TimedJobEndedEvent(TimedJobState timedJob, Vector2I location) : IEvent
{
    public TimedJobState timedJob { get; private set; } = timedJob;
    public Vector2I location { get; private set; } = location;
}
