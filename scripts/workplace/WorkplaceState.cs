using System.Collections.Generic;
using Godot;

public class WorkplaceState(Vector2I location, CustomTileData tileData, Texture2D iconTexture)
{
    public Vector2I location { get; private set; } = location;
    public CustomTileData tileData { get; private set; } = tileData;
    public int capacity => tileData.workerCapacity;
    public string name => tileData.GetFileName();
    public Texture2D iconTexture { get; private set; } = iconTexture;

    private List<ResidentState> _workers = new();
    public ResidentState[] workers => _workers.ToArray();
    public int workerCount => workers.Length;

    public bool TryAddWorker(ResidentState resident)
    {
        if (_workers.Count >= capacity)
            return false;

        _workers.Add(resident);
        return true;
    }

    public bool TryRemoveWorker(out ResidentState resident)
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

    public void UpdateWorkplaceType(CustomTileData newTileData, out List<ResidentState> removedWorkers)
    {
        removedWorkers = new();

        if (capacity <= newTileData.workerCapacity)
        {
            tileData = newTileData;
            return;
        }

        //get sublist of _workers from {newCapacity}th index to end
        removedWorkers.AddRange(_workers[newTileData.workerCapacity..]);
        _workers.RemoveRange(newTileData.workerCapacity, capacity - newTileData.workerCapacity);
        tileData = newTileData;
    }

    public CurrencySum GetWorkerDependentAdjacencyEffects(CustomTileData affectedTileData)
    {
        if (_workers.Count == 0)
            return new();

        bool hasEffect = affectedTileData.TryGetAdjacencyEffectFromTileData(tileData, out var baseEffectPerWorker);

        return !hasEffect ? new() : baseEffectPerWorker * _workers.Count;

    }
}