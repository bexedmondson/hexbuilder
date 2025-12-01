using System.Collections.Generic;
using Godot;

public class HouseState
{
    public Vector2I location { get; private set; }
    public int capacity { get; private set; }
    private List<ResidentState> _occupants = new();
    public ResidentState[] occupants => _occupants.ToArray();
    
    public HouseState(Vector2I location, int capacity)
    {
        this.location = location;
        this.capacity = capacity;
    }

    public Vector2 GetCentreWorldPosition()
    {
        var mapController = InjectionManager.Get<MapController>();
        return mapController.BaseMapLayer.ToGlobal(mapController.BaseMapLayer.MapToLocal(location));
    }

    public bool TryAddOccupant(ResidentState resident)
    {
        if (_occupants.Count >= capacity)
            return false;
        
        _occupants.Add(resident);
        return true;
    }

    public void ChangeCapacity(int newCapacity, out List<ResidentState> kickedOutResidents)
    {
        kickedOutResidents = new();
        
        if (capacity <= newCapacity)
        {
            capacity = newCapacity;
            return;
        }
        
        //get sublist of _occupants from {newCapacity}th index to end
        kickedOutResidents.AddRange(_occupants[newCapacity..]);
        _occupants.RemoveRange(newCapacity, capacity - newCapacity);
        capacity = newCapacity;
    }
}
