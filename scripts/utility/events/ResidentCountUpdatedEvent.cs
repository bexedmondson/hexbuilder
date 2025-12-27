public class ResidentCountUpdatedEvent(int count) : IEvent
{
    public int count { get; private set; } = count;
}
