
using System;
using System.Collections.Generic;
using Godot;

public class ResidentManager : IInjectable
{
    private MapController mapController;
    private HousingManager housingManager;

    private List<ResidentData> residents = new();
    public ResidentData[] AllResidents => residents.ToArray();

    private string[] names;
    
    public ResidentManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);

        string json = System.IO.File.ReadAllText("data/residents/firstNames.json");
        names = System.Text.Json.JsonSerializer.Deserialize<string[]>(json);
    }

    public void OnNewGame()
    {
        housingManager ??= InjectionManager.Get<HousingManager>();
    }

    public void OnNextTurn()
    {
        foreach (var resident in residents)
        {
            if (!resident.HasHouse)
                return;
        }

        //if no food, return
        if (!InjectionManager.Get<InventoryManager>().CanAfford(new CurrencySum(CurrencyType.Food, 1)))
            return;

        CreateResident();
    }

    public ResidentData CreateResident()
    {
        var newResident = new ResidentData(names[GD.RandRange(0, names.Length - 1)]);
        residents.Add(newResident);

        UpdateResidentHousing();
        
        return newResident;
    }

    private void UpdateResidentHousing(MapUpdatedEvent e = null)
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
}
