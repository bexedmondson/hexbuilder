using Godot;

public partial class ResidentNoHomeWarningIndicator : Control
{
    private ResidentManager residentManager;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Get<EventDispatcher>().Add<ResidentHouseStateUpdateedEvent>(OnResidentHousingStateUpdated);
    }

    private void OnResidentHousingStateUpdated(ResidentHouseStateUpdateedEvent e)
    {
        residentManager ??= InjectionManager.Get<ResidentManager>();

        this.Visible = residentManager.DoesResidentExistWithoutHouse();
    }
}
