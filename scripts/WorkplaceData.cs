using System.Collections.Generic;
using Godot;

public class WorkplaceData(Vector2I location, int capacity, string name, Texture iconTexture)
{
    public Vector2I location { get; private set; } = location;
    public int capacity { get; private set; } = capacity;
    public string name { get; private set; } = name;
    public Texture iconTexture { get; private set; } = iconTexture;

    private List<ResidentData> _workers = new();
    public ResidentData[] workers => _workers.ToArray();

    public Vector2 GetCentreWorldPosition()
    {
        var mapController = InjectionManager.Get<MapController>();
        return mapController.BaseMapLayer.ToGlobal(mapController.BaseMapLayer.MapToLocal(location));
    }

    public bool TryAddWorker(ResidentData resident)
    {
        if (_workers.Count >= capacity)
            return false;
        
        _workers.Add(resident);
        return true;
    }

    public void ChangeCapacity(int newCapacity, out List<ResidentData> removedWorkers)
    {
        removedWorkers = new();
        
        if (capacity <= newCapacity)
        {
            capacity = newCapacity;
            return;
        }
        
        //get sublist of _workers from {newCapacity}th index to end
        removedWorkers.AddRange(_workers[newCapacity..]);
        _workers.RemoveRange(newCapacity, capacity - newCapacity);
        capacity = newCapacity;
    }
}