public class AutoUpgradeManager : IInjectable
{
    private MapController mapController;
    
    public AutoUpgradeManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }
    
    ~AutoUpgradeManager()
    {
        InjectionManager.Deregister(this);
    }

    public void OnNextTurn()
    {
        //check through used cells for auto upgrade component
    }
}
