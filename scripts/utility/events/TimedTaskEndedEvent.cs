using Godot;

public class TimedTaskEndedEvent(TimedTaskState timedTask, Vector2I location) : IEvent
{
    public TimedTaskState timedTask { get; private set; } = timedTask;
    public Vector2I location { get; private set; } = location;
}
