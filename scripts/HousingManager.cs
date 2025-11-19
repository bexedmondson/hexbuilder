
public class HousingManager : IInjectable
{
    private MapController mapController;
    private TileDatabase tileDatabase;
    
    public HousingManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNewGame()
    {
        tileDatabase = InjectionManager.Get<TileDatabase>();
    }
}
