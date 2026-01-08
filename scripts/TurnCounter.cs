public class TurnCounter : IInjectable
{
    public int turnCount { get; private set; } = 1;

    public TurnCounter()
    {
        InjectionManager.Register(this);
    }

    ~TurnCounter()
    {
        InjectionManager.Deregister(this);
    }
    
    public void OnNextTurn()
    {
        turnCount++;
    }
}
