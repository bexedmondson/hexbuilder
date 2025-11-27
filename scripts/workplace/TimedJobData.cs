using System.Collections.Generic;
using Godot;

public abstract class TimedJobData(Vector2I location, int turnDuration, int workerCountRequirement)
{
    public Vector2I location { get; private set; } = location;

    public abstract string description { get; }

    private List<ResidentData> _workers = new();
    public ResidentData[] workers => _workers.ToArray();
    public int workerCount => workers.Length;
    private int workerCountRequirement = workerCountRequirement;
    
    public int turnDuration { get; private set; } = turnDuration;
    public int turnsElapsed { get; private set; }
    public bool HasFinished => turnsElapsed >= turnDuration;

    public bool TryIncrementTurnCount()
    {
        //timed jobs must be fully staffed to proceed
        if (workerCount < workerCountRequirement)
            return false;
        
        turnsElapsed++;
        return true;
    }
    
    public bool TryAddWorker(ResidentData resident)
    {
        if (_workers.Count >= workerCountRequirement)
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

    public abstract void CompleteJob();
}
