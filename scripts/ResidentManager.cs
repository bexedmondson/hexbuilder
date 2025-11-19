
public class ResidentManager : IInjectable
{
    private MapController mapController;
    
    public ResidentManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNewGame()
    {
        
    }
}
