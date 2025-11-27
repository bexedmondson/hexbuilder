
public class ResidentData(string name)
{
    private HousingManager housingManager;
    
    public string Name { get; private set; } = name;

    public int TurnsWithoutHouse { get; private set; } = 0;

    public bool HasHouse
    {
        get
        {
            housingManager ??= InjectionManager.Get<HousingManager>();
            return housingManager.TryGetHouseForResident(this, out _);
        }
    }

    public void IncrementNoHouseTracking()
    {
        TurnsWithoutHouse++;
    }

    public void ResetNoHouseTracking()
    {
        TurnsWithoutHouse = 0;
    }
    
    private WorkplaceManager workplaceManager;
    
    public bool IsBusy
    {
        get
        {
            workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
            return workplaceManager.TryGetWorkplaceForResident(this, out _);
        }
    }

    public string GetWorkplaceName()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        if (workplaceManager.TryGetWorkplaceForResident(this, out var workplace))
        {
            return workplace.name;
        }
        else
        {
            return string.Empty;
        }
    }
}
