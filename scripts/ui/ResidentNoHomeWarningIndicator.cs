using Godot;

public partial class ResidentNoHomeWarningIndicator : Control
{
    private ResidentManager residentManager;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Get<EventDispatcher>().Add<ResidentHouseStateUpdatedEvent>(OnResidentHousingStateUpdated);
    }

    private void OnResidentHousingStateUpdated(ResidentHouseStateUpdatedEvent e)
    {
        residentManager ??= InjectionManager.Get<ResidentManager>();

        this.Visible = residentManager.DoesResidentExistWithoutHouse();
    }
}
