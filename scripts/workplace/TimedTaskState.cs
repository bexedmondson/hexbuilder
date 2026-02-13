using System.Collections.Generic;
using Godot;

public abstract class TimedTaskState(Vector2I location, int turnDuration, int workerCountRequirement = 0)
{
    public Vector2I location { get; private set; } = location;

    public abstract string description { get; }

    protected List<ResidentState> _workers = new();
    public ResidentState[] workers => _workers.ToArray();
    public int workerCount => workers.Length;
    public virtual int workerCountRequirement { get; protected set; } = workerCountRequirement;
    
    public int turnDuration { get; } = turnDuration;
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
    
    public bool TryAddWorker(ResidentState resident)
    {
        if (_workers.Count >= workerCountRequirement)
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

    public abstract void CompleteJob();
}
