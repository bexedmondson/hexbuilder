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
    
    public ResidentManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);

        needAssignmentManager = new(this);

        string json = System.IO.File.ReadAllText("data/residents/firstNames.json");
        names = System.Text.Json.JsonSerializer.Deserialize<string[]>(json);
    }

    public void OnNewGame()
    {
        housingManager ??= InjectionManager.Get<HousingManager>();
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        InjectionManager.Get<EventDispatcher>().Add<HouseUpdatedEvent>(UpdateResidentHousing);
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

        CreateResident();
    }

    public ResidentState CreateResident()
    {
        turnCounter ??= InjectionManager.Get<TurnCounter>();
        var newResident = new ResidentState(names[GD.RandRange(0, names.Length - 1)], turnCounter.turnCount);
        residents.Add(newResident);
        
        InjectionManager.Get<EventDispatcher>().Dispatch(new ResidentCountUpdatedEvent(residents.Count));

        UpdateResidentHousing();
        
        InjectionManager.Get<ToastManager>().RequestToast(new ToastConfig{text = $"new resident: {newResident.Name}"}, "new_resident");
        
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
