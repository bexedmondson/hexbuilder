using Godot;

public class ResidentNeedAssignmentManager : IInjectable
{
    private ResidentManager residentManager;
    private NeedConfigList needConfigList;
    
    public ResidentNeedAssignmentManager(ResidentManager residentManager)
    {
        this.residentManager = residentManager;
        InjectionManager.Register(this);
    }

    ~ResidentNeedAssignmentManager()
    {
        InjectionManager.Deregister(this);
    }

    public void UpdateNeedsAssignment(ResidentState residentState)
    {
        needConfigList ??= InjectionManager.Get<DataResourceContainer>().needConfigList;

        foreach (var resident in residentManager.AllResidents)
        {
            foreach (var needConfig in needConfigList.needConfigs)
            {
                if (needConfig.CanAssignToResident(resident))
                {
                    GD.Print($"Assigning new active need {needConfig.satisfactionRequirements[0].GetType()} to {resident.Name}");
                    resident.activeNeeds.Add(needConfig);
                }
            }
        }

        residentState.UpdateHappiness();
    }
}
