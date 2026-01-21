
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HousingManager : IInjectable
{
    private MapController mapController;
    private TileDatabase tileDatabase;

    private Dictionary<Vector2I, HouseState> houseDatas = new();
    public HouseState[] AllHouseDatas => houseDatas.Values.ToArray();

    private Dictionary<ResidentState, HouseState> residentHousingMap = new();
    
    public bool AllHousingFull {
        get
        {
            foreach (var houseData in houseDatas)
            {
                if (!houseData.Value.IsFull)
                    return false;
            }

            return true;
        }
    }

    public int TotalFreeHousingSpaces
    {
        get
        {
            int count = 0;
            foreach (var houseData in houseDatas)
            {
                count += houseData.Value.capacity - houseData.Value.occupants.Length;
            }

            return count;
        }
    }
    
    public HousingManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNewGame()
    {
        tileDatabase = InjectionManager.Get<TileDatabase>();

        OnMapUpdated(null);
        InjectionManager.Get<EventDispatcher>().Add<MapUpdatedEvent>(OnMapUpdated);
    }

    private void OnMapUpdated(MapUpdatedEvent _)
    {
        bool housesChanged = false;
        var usedCells = mapController.BaseMapLayer.GetUsedCells();
        
        List<Vector2I> existingMapHouseCoords = new();
        
        foreach (var cell in usedCells)
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;
            
            var cellData = mapController.BaseMapLayer.GetCellCustomData(cell);
            
            if (cellData == null)
            {
                GD.Print($"cell at {cell} has null custom data??");
                continue;
            }
                
            if (!cellData.TryGetComponent<ResidentCapacityComponent>(out var cellDataResidentCapacity) 
                || cellDataResidentCapacity.capacity == 0)
                continue;
            
            existingMapHouseCoords.Add(cell);

            if (houseDatas.TryGetValue(cell, out var existingHouseData))
            {
                existingHouseData.UpdateResidenceType(cellData, out List<ResidentState> kickedOutResidents);

                foreach (var kickedOutResident in kickedOutResidents)
                {
                    residentHousingMap.Remove(kickedOutResident);
                    InjectionManager.Get<EventDispatcher>().Dispatch(new ResidentHouseStateUpdatedEvent(kickedOutResident));
                }
                
                housesChanged = true;

            }
            else
            {
                houseDatas[cell] = new HouseState(cell, cellData);
                housesChanged = true;
            }
        }

        foreach (var houseData in houseDatas)
        {
            if (existingMapHouseCoords.Contains(houseData.Key))
                continue;

            housesChanged = true;
        }

        if (housesChanged)
        {
            InjectionManager.Get<EventDispatcher>().Dispatch(new HouseUpdatedEvent());
        }
    }

    public bool TryAssignResidentToHouse(ResidentState resident)
    {
        List<HouseState> availableHouses = new();
        foreach (var houseData in AllHouseDatas)
        {
            if (houseData.occupants.Length < houseData.capacity)
                availableHouses.Add(houseData);
        }

        if (availableHouses.Count == 0)
            return false;

        var chosenHouse = availableHouses[GD.RandRange(0, availableHouses.Count - 1)];
        chosenHouse.TryAddOccupant(resident);
        
        residentHousingMap[resident] = chosenHouse; //TODO consider removing duplicate data here? update map from HouseData method?? hmm
        
        InjectionManager.Get<EventDispatcher>().Dispatch(new ResidentHouseStateUpdatedEvent(resident));
        return true;
    }

    public bool TryGetHouseForResident(ResidentState resident, out HouseState houseState)
    {
        return residentHousingMap.TryGetValue(resident, out houseState);
    }

    public bool TryGetHouseOnCell(Vector2I cell, out HouseState houseState)
    {
        return houseDatas.TryGetValue(cell, out houseState);
    }
}
