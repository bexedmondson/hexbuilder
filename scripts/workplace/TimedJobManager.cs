using System.Collections.Generic;
using System.Linq;
using Godot;

public class TimedJobManager : IInjectable
{
    private MapController mapController;
    private EventDispatcher eventDispatcher;

    private Dictionary<Vector2I, TimedJobData> timedJobDatas = new();

    private Dictionary<ResidentData, TimedJobData> residentTimedJobMap = new();
    
    public TimedJobManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNextTurn()
    {
        List<Vector2I> locationsOfJobsToRemove = new();
        foreach (var kvp in timedJobDatas)
        {
            var timedJobData = kvp.Value;
            timedJobData.TryIncrementTurnCount();

            if (!timedJobData.HasFinished)
                return;
            
            timedJobData.CompleteJob();
            foreach (var worker in timedJobData.workers)
            {
                residentTimedJobMap.Remove(worker);
            }
            locationsOfJobsToRemove.Add(kvp.Key);
        }

        foreach (var locationOfJobToRemove in locationsOfJobsToRemove)
        {
            timedJobDatas.Remove(locationOfJobToRemove);
            
            eventDispatcher ??= InjectionManager.Get<EventDispatcher>();
            eventDispatcher.Dispatch(new TimedJobEndedEvent(locationOfJobToRemove));
        }
    }

    public void AddNewTimedJob(TimedJobData newTimedJob)
    {
        timedJobDatas.Add(newTimedJob.location, newTimedJob);
        
        eventDispatcher ??= InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Dispatch(new TimedJobStartedEvent(newTimedJob.location));
    }

    public bool TryGetTimedJobAt(Vector2I cell, out TimedJobData timedJobData)
    {
        return timedJobDatas.TryGetValue(cell, out timedJobData);
    }

    public bool TryAssignResidentToTimedJob(TimedJobData timedJobData)
    {
        ResidentManager residentManager = InjectionManager.Get<ResidentManager>();
        if (!residentManager.TryGetFirstNotBusyResident(out var chosenResident))
            return false;

        if (!timedJobData.TryAddWorker(chosenResident))
            return false;
        
        residentTimedJobMap[chosenResident] = timedJobData; //TODO consider removing duplicate data here? update map from TimedJobData method?? hmm
        return true;
    }

    public bool TryRemoveResidentFromTimedJob(TimedJobData timedJobData)
    {
        if (timedJobData.workerCount == 0)
            return false;

        if (!timedJobData.TryRemoveWorker(out var resident))
            return false;

        residentTimedJobMap.Remove(resident);
        return true;
    }

    public bool TryGetTimedJobForResident(ResidentData resident, out TimedJobData timedJobData)
    {
        return residentTimedJobMap.TryGetValue(resident, out timedJobData);
    }
}
