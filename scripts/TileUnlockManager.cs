using Godot;

public partial class TileUnlockManager : IInjectable
{
    private TileDatabase tileDatabase;
    
    public TileUnlockManager(TileDatabase tileDatabase)
    {
        this.tileDatabase = tileDatabase;
        InjectionManager.Register(this);
    }

    ~TileUnlockManager()
    {
        InjectionManager.Deregister(this);
    }
}
