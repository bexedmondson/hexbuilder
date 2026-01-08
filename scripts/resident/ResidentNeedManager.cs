public class ResidentNeedManager : IInjectable
{
    private ResidentManager residentManager;
    private NeedConfigList needConfigList;
    
    public ResidentNeedManager(ResidentManager residentManager)
    {
        this.residentManager = residentManager;
        InjectionManager.Register(this);
    }

    ~ResidentNeedManager()
    {
        InjectionManager.Deregister(this);
    }

    public void UpdateNeeds(ResidentState residentState)
    {
        needConfigList ??= InjectionManager.Get<DataResourceContainer>().needConfigList;

        foreach (var resident in residentManager.AllResidents)
        {
            foreach (var needConfig in needConfigList.needConfigs)
            {
                
            }
        }
        
        //for each resident
        //for each need in need resource list
        //if need condition met, assign need
        //e.g. if resident has lived in town for X days, and if random chance is > 50%, assign need to live in house not tent 
    }
}
