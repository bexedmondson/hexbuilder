using System.Collections.Generic;
using System.Linq;
using Godot;

public class TimedTaskManager : IInjectable
{
    private MapController mapController;
    private EventDispatcher eventDispatcher;

    private Dictionary<Vector2I, TimedTaskState> timedTaskDatas = new();

    private Dictionary<ResidentState, TimedTaskState> residentTimedTaskMap = new();
    
    public TimedTaskManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNextTurn()
    {
        List<KeyValuePair<Vector2I, TimedTaskState>> timedTaskLocationPairsToRemove = new();
        foreach (var kvp in timedTaskDatas)
        {
            var timedTaskData = kvp.Value;
            timedTaskData.TryIncrementTurnCount();

            if (!timedTaskData.HasFinished)
            {
                eventDispatcher.Dispatch(new TimedTaskUpdatedEvent(timedTaskData, kvp.Key));
                return;
            }
            
            timedTaskData.CompleteJob();
            foreach (var worker in timedTaskData.workers)
            {
                residentTimedTaskMap.Remove(worker);
            }
            timedTaskLocationPairsToRemove.Add(kvp);
        }

        foreach (var jobLocationPairToRemove in timedTaskLocationPairsToRemove)
        {
            GD.Print($"TimedTaskManager: removing timed task {jobLocationPairToRemove.Value.description} at {jobLocationPairToRemove.Key}");
            timedTaskDatas.Remove(jobLocationPairToRemove.Key);
            mapController.OnTimedTaskEnded(jobLocationPairToRemove.Key);
            
            eventDispatcher ??= InjectionManager.Get<EventDispatcher>();
            eventDispatcher.Dispatch(new TimedTaskEndedEvent(jobLocationPairToRemove.Value, jobLocationPairToRemove.Key));
        }
    }

    public void AddNewTimedTask(TimedTaskState newTimedTask)
    {
        GD.Print($"TimedTaskManager: creating timed task {newTimedTask.description} at {newTimedTask.location}");
        timedTaskDatas.Add(newTimedTask.location, newTimedTask);
        
        //TODO consider if this is what should always happen or not
        mapController.SetCellBusy(newTimedTask.location);
        
        eventDispatcher ??= InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Dispatch(new TimedTaskStartedEvent(newTimedTask));
    }

    public bool TryGetTimedTaskAt(Vector2I cell, out TimedTaskState timedTaskState)
    {
        return timedTaskDatas.TryGetValue(cell, out timedTaskState);
    }

    public bool TryAssignResidentToTimedTask(TimedTaskState timedTaskState)
    {
        ResidentManager residentManager = InjectionManager.Get<ResidentManager>();
        if (!residentManager.TryGetFirstNotBusyResident(out var chosenResident))
            return false;

        if (!timedTaskState.TryAddWorker(chosenResident))
            return false;
        
        residentTimedTaskMap[chosenResident] = timedTaskState; //TODO consider removing duplicate data here? update map from TimedTaskData method?? hmm
        return true;
    }

    public bool TryRemoveResidentFromTimedTask(TimedTaskState timedTaskState)
    {
        if (timedTaskState.workerCount == 0)
            return false;

        if (!timedTaskState.TryRemoveWorker(out var resident))
            return false;

        residentTimedTaskMap.Remove(resident);
        return true;
    }

    public bool TryGetTimedTaskForResident(ResidentState resident, out TimedTaskState timedTaskState)
    {
        return residentTimedTaskMap.TryGetValue(resident, out timedTaskState);
    }
}
