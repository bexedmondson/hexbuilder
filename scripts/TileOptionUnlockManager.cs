using Godot;

public partial class TileOptionUnlockManager : IInjectable
{
    private TileDatabase tileDatabase;
    
    public TileOptionUnlockManager(TileDatabase tileDatabase)
    {
        this.tileDatabase = tileDatabase;
        InjectionManager.Register(this);
    }

    ~TileOptionUnlockManager()
    {
        InjectionManager.Deregister(this);
    }
}
