
using System.Collections.Generic;
using Godot;

public class HouseData
{
    public Vector2I location { get; private set; }
    public int capacity { get; private set; }
    private List<ResidentData> _occupants = new();
    public ResidentData[] occupants => _occupants.ToArray();
    
    public HouseData(Vector2I location, int capacity)
    {
        this.location = location;
        this.capacity = capacity;
    }

    public Vector2 GetCentreWorldPosition()
    {
        var mapController = InjectionManager.Get<MapController>();
        return mapController.BaseMapLayer.ToGlobal(mapController.BaseMapLayer.MapToLocal(location));
    }
}
