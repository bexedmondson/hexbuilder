using System.Collections.Generic;
using Godot;

public class HouseState
{
    public Vector2I location { get; private set; }
    public CustomTileData tileData { get; private set; }
    public int capacity => tileData.TryGetComponent<ResidentCapacityComponent>(out var residentCapacity) ? residentCapacity.capacity : 0;
    
    private List<ResidentState> _occupants = new();
    public ResidentState[] occupants => _occupants.ToArray();

    public bool IsFull => capacity <= _occupants.Count;
    
    public HouseState(Vector2I location, CustomTileData customTileData)
    {
        this.location = location;
        this.tileData = customTileData;
    }

    public bool TryAddOccupant(ResidentState resident)
    {
        if (_occupants.Count >= capacity)
            return false;
        
        _occupants.Add(resident);
        return true;
    }

    public void UpdateResidenceType(CustomTileData newTileData, out List<ResidentState> kickedOutResidents)
    {
        kickedOutResidents = new();

        var newTileDataHasResidentCapacity = newTileData.TryGetComponent<ResidentCapacityComponent>(out var newResidentCapacity);
        
        //if we increased the resident capacity on this tile, save the new tile data and carry on
        if (newTileDataHasResidentCapacity && capacity <= newResidentCapacity.capacity)
        {
            tileData = newTileData;
            return;
        }

        var newCapacity = newTileDataHasResidentCapacity ? newResidentCapacity.capacity : 0;
        
        //get sublist of _occupants from {newCapacity}th index to end
        kickedOutResidents.AddRange(_occupants[newCapacity..]);
        _occupants.RemoveRange(newCapacity, capacity - newCapacity);
        tileData = newTileData;
    }
}
