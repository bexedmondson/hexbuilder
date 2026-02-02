using System.Collections.Generic;
using Godot;

public class ResidentManager : IInjectable
{
    private MapController mapController;
    private HousingManager housingManager;
    private WorkplaceManager workplaceManager;
    private TurnCounter turnCounter;

    private ResidentNeedAssignmentManager needAssignmentManager;

    private List<ResidentState> residents = new();
    public ResidentState[] AllResidents => residents.ToArray();

    private string[] names;
    private List<string> unusedNames = new();
    
    public ResidentManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);

        needAssignmentManager = new(this);

        var json = InjectionManager.Get<DataResourceContainer>().residentNames;
        names = json.Data.AsStringArray();
    }

    public void OnNewGame()
    {
        housingManager ??= InjectionManager.Get<HousingManager>();
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        InjectionManager.Get<EventDispatcher>().Add<HouseUpdatedEvent>(UpdateResidentHousing);
        
        RefreshUnusedNamesList();
    }

    private void RefreshUnusedNamesList()
    {
        unusedNames.Clear();

        if (residents.Count > names.Length) //if more residents than names, just let selection happen randomly
        {
            unusedNames.AddRange(names);
            return;
        }
        
        foreach (string name in names)
        {
            if (residents.Exists(resident => resident.Name == name))
                continue;
            unusedNames.Add(name);
        }
    }

    public void OnNextTurn()
    {
        bool residentWithoutHouse = false;
        foreach (var resident in residents)
        {
            needAssignmentManager.UpdateNeedsAssignment(resident);
            if (!resident.HasHouse)
                residentWithoutHouse = true;
        }

        if (residentWithoutHouse)
            return;

        //if no food, return
        if (!InjectionManager.Get<InventoryManager>().CanAfford(new CurrencySum(CurrencyType.Food, residents.Count)))
            return;

        if (InjectionManager.Get<HousingManager>().AllHousingFull)
            return;

        int spacesFree = InjectionManager.Get<HousingManager>().TotalFreeHousingSpaces;
        for (int i = 0; i < spacesFree; i++)
        {
            CreateResident();
        }
    }

    public ResidentState CreateResident()
    {
        turnCounter ??= InjectionManager.Get<TurnCounter>();
        var newResident = new ResidentState(unusedNames[GD.RandRange(0, unusedNames.Count - 1)], turnCounter.turnCount);
        if (residents.Count > names.Length)
            RefreshUnusedNamesList();
        else
            unusedNames.Remove(newResident.Name);
        
        residents.Add(newResident);
        
        InjectionManager.Get<EventDispatcher>().Dispatch(new ResidentCountUpdatedEvent(residents.Count));

        UpdateResidentHousing();
        
        InjectionManager.Get<ToastManager>().RequestToast(new ToastConfig{text = $"new resident: {newResident.Name}", stackId = "new_resident"});
        
        return newResident;
    } 

    private void UpdateResidentHousing(IEvent e = null)
    {
        foreach (var resident in residents)
        {
            if (!resident.HasHouse)
            {
                bool foundHouse = housingManager.TryAssignResidentToHouse(resident);

                if (!foundHouse)
                    resident.IncrementNoHouseTracking();
            }
            else if (resident.TurnsWithoutHouse > 0)
            {
                resident.ResetNoHouseTracking();
            }
        }
    }

    public ResidentState[] GetNotBusyResidents()
    {
        List<ResidentState> unemployedResidents = new();
        foreach (var resident in residents)
        {
            if (!resident.IsBusy)
                unemployedResidents.Add(resident);
        }
        return unemployedResidents.ToArray();
    }

    public int GetNotBusyResidentCount()
    {
        int count = 0;
        foreach (var resident in residents)
        {
            if (!resident.IsBusy)
                count++;
        }
        return count;
    }
    
    public bool TryGetFirstNotBusyResident(out ResidentState residentState)
    {
        foreach (var resident in residents)
        {
            if (!resident.IsBusy)
            {
                residentState = resident;
                return true;
            }
        }
        residentState = null;
        return false;
    }
    
    public bool DoesResidentExistWithoutHouse()
    {
        foreach (var resident in AllResidents)
        {
            if (!resident.HasHouse)
                return true;
        }

        return false;
    }
}
