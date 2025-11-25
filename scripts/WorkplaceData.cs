using System.Collections.Generic;
using Godot;

public class WorkplaceData(Vector2I location, int capacity, string name, Texture2D iconTexture)
{
    public Vector2I location { get; private set; } = location;
    public int capacity { get; private set; } = capacity;
    public string name { get; private set; } = name;
    public Texture2D iconTexture { get; private set; } = iconTexture;

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
    
    public bool TryRemoveWorker(out ResidentData resident)
    {
        if (_workers.Count <= 0)
        {
            resident = null;
            return false;
        }

        resident = _workers[^1];
        _workers.RemoveAt(_workers.Count - 1);
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