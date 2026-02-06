using System.Collections.Generic;
using System.Linq;
using Godot;

public class TimedJobManager : IInjectable
{
    private MapController mapController;
    private EventDispatcher eventDispatcher;

    private Dictionary<Vector2I, TimedJobState> timedJobDatas = new();

    private Dictionary<ResidentState, TimedJobState> residentTimedJobMap = new();
    
    public TimedJobManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNextTurn()
    {
        List<KeyValuePair<Vector2I, TimedJobState>> timedJobLocationPairsToRemove = new();
        foreach (var kvp in timedJobDatas)
        {
            var timedJobData = kvp.Value;
            timedJobData.TryIncrementTurnCount();

            if (!timedJobData.HasFinished)
            {
                eventDispatcher.Dispatch(new TimedJobUpdatedEvent(timedJobData, kvp.Key));
                return;
            }
            
            timedJobData.CompleteJob();
            foreach (var worker in timedJobData.workers)
            {
                residentTimedJobMap.Remove(worker);
            }
            timedJobLocationPairsToRemove.Add(kvp);
        }

        foreach (var jobLocationPairToRemove in timedJobLocationPairsToRemove)
        {
            timedJobDatas.Remove(jobLocationPairToRemove.Key);
            
            eventDispatcher ??= InjectionManager.Get<EventDispatcher>();
            eventDispatcher.Dispatch(new TimedJobEndedEvent(jobLocationPairToRemove.Value, jobLocationPairToRemove.Key));
        }
    }

    public void AddNewTimedJob(TimedJobState newTimedJob)
    {
        timedJobDatas.Add(newTimedJob.location, newTimedJob);
        
        eventDispatcher ??= InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Dispatch(new TimedJobStartedEvent(newTimedJob, newTimedJob.location, newTimedJob.workerCount, newTimedJob.workerCountRequirement));
    }

    public bool TryGetTimedJobAt(Vector2I cell, out TimedJobState timedJobState)
    {
        return timedJobDatas.TryGetValue(cell, out timedJobState);
    }

    public bool TryAssignResidentToTimedJob(TimedJobState timedJobState)
    {
        ResidentManager residentManager = InjectionManager.Get<ResidentManager>();
        if (!residentManager.TryGetFirstNotBusyResident(out var chosenResident))
            return false;

        if (!timedJobState.TryAddWorker(chosenResident))
            return false;
        
        residentTimedJobMap[chosenResident] = timedJobState; //TODO consider removing duplicate data here? update map from TimedJobData method?? hmm
        return true;
    }

    public bool TryRemoveResidentFromTimedJob(TimedJobState timedJobState)
    {
        if (timedJobState.workerCount == 0)
            return false;

        if (!timedJobState.TryRemoveWorker(out var resident))
            return false;

        residentTimedJobMap.Remove(resident);
        return true;
    }

    public bool TryGetTimedJobForResident(ResidentState resident, out TimedJobState timedJobState)
    {
        return residentTimedJobMap.TryGetValue(resident, out timedJobState);
    }
}
