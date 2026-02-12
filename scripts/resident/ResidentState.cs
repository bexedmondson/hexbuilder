
using System.Collections.Generic;
using Godot;

public class ResidentState {
    private HousingManager housingManager;

    public string Name { get; private set; }

    public List<NeedConfig> activeNeeds = new();

    public int moveInDay { get; private set; }

    public ResidentState(){} //adding this purely so it's compatible with the generics stuff in IsNeedSatisfied below

    public ResidentState(string name, int moveInDay)
    {
        this.Name = name;
        this.moveInDay = moveInDay;
    }

    public static int maxHappiness = 3;
    public static int minHappiness = -3;
    
    public int happiness { get; private set; } = 0;
    public void UpdateHappiness()
    {
        int prev = happiness;
        int happinessSum = 0;
        foreach (var activeNeed in activeNeeds)
        {
            if (activeNeed.IsNeedSatisfied(this))
                happinessSum += activeNeed.satisfiedHappinessBonus;
            else
                happinessSum += activeNeed.unsatisfiedHappinessPenalty;
        }

        happiness = happinessSum;
        happiness = Mathf.Min(maxHappiness, happiness);
        happiness = Mathf.Max(minHappiness, happiness);
        
        if (happiness != prev)
        {
            string notification = $"{Name} {(happiness > prev ? "is feeling happier!" : "is feeling sadder...")}";
            InjectionManager.Get<ToastManager>().RequestToast(new ToastConfig{text = notification, stackId = "happiness"});
        }
    }

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
    private TimedTaskManager timedTaskManager;
    
    public bool IsBusy
    {
        get
        {
            workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
            if (workplaceManager.TryGetWorkplaceForResident(this, out _))
                return true;

            timedTaskManager ??= InjectionManager.Get<TimedTaskManager>();
            if (timedTaskManager.TryGetTimedTaskForResident(this, out _))
                return true;

            return false;
        }
    }

    public string GetWorkplaceOrJobName()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        timedTaskManager ??= InjectionManager.Get<TimedTaskManager>();
        if (workplaceManager.TryGetWorkplaceForResident(this, out var workplace))
        {
            return workplace.name;
        }
        else if (timedTaskManager.TryGetTimedTaskForResident(this, out var timedTask))
        {
            return timedTask.description;
        }
        else
        {
            return string.Empty;
        }
    }
}
