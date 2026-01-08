
using Godot;

public class ResidentState(string name, int moveInDay)
{
    private HousingManager housingManager;
    
    public string Name { get; private set; } = name;

    public int moveInDay { get; private set; } = moveInDay;
    
    public int happiness { get; private set; } = 1;

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
        bool emit = TurnsWithoutHouse == 0;
        
        TurnsWithoutHouse++;
        
        if (emit)
            InjectionManager.Get<EventDispatcher>().Dispatch(new ResidentHouseStateUpdatedEvent(this));
    }

    public void ResetNoHouseTracking()
    {
        TurnsWithoutHouse = 0;
    }
    
    private WorkplaceManager workplaceManager;
    private TimedJobManager timedJobManager;
    
    public bool IsBusy
    {
        get
        {
            workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
            if (workplaceManager.TryGetWorkplaceForResident(this, out _))
                return true;

            timedJobManager ??= InjectionManager.Get<TimedJobManager>();
            if (timedJobManager.TryGetTimedJobForResident(this, out _))
                return true;

            return false;
        }
    }

    public string GetWorkplaceOrJobName()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        timedJobManager ??= InjectionManager.Get<TimedJobManager>();
        if (workplaceManager.TryGetWorkplaceForResident(this, out var workplace))
        {
            return workplace.name;
        }
        else if (timedJobManager.TryGetTimedJobForResident(this, out var timedJob))
        {
            return timedJob.description;
        }
        else
        {
            return string.Empty;
        }
    }

    public void ChangeHappiness(int delta)
    {
        happiness = Mathf.Max(happiness + delta, -3);
        happiness = Mathf.Min(happiness + delta, 3);
    }
}
