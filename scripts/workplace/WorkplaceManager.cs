using System.Collections.Generic;
using System.Linq;
using Godot;

public class WorkplaceManager : IInjectable
{
    private MapController mapController;
    private TileDatabase tileDatabase;

    private Dictionary<Vector2I, WorkplaceState> workplaceDatas = new();
    public WorkplaceState[] AllWorkplaceDatas => workplaceDatas.Values.ToArray();

    private Dictionary<ResidentState, WorkplaceState> residentWorkplaceMap = new();
    
    public WorkplaceManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNewGame()
    {
        tileDatabase = InjectionManager.Get<TileDatabase>();

        OnMapUpdated(null);
        InjectionManager.Get<EventDispatcher>().Add<MapUpdatedEvent>(OnMapUpdated);
    }

    private void OnMapUpdated(MapUpdatedEvent _)
    {
        WorkplaceUpdatedEvent workplaceUpdatedEvent = new();
        var usedCells = mapController.BaseMapLayer.GetUsedCells();
        
        List<Vector2I> existingMapWorkplaceCoords = new();
        
        foreach (var cell in usedCells)
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;
            
            var cellData = mapController.BaseMapLayer.GetCellCustomData(cell);
            
            if (cellData.workerCapacity == 0)
                continue;
            
            existingMapWorkplaceCoords.Add(cell);

            if (workplaceDatas.TryGetValue(cell, out var existingWorkplaceData))
            {
                if (existingWorkplaceData.tileData == cellData)
                    continue;

                existingWorkplaceData.UpdateWorkplaceType(cellData, out List<ResidentState> removedWorkers);

                foreach (var removedWorker in removedWorkers)
                {
                    residentWorkplaceMap.Remove(removedWorker);
                }
                
                workplaceUpdatedEvent.newOrChangedWorkplaces.Add(existingWorkplaceData);
            }
            else
            {
                //TODO pass in CustomTileData instead?
                var newWorkplace = new WorkplaceState(cell, cellData, tileDatabase.GetTileTexture(cellData));
                workplaceDatas[cell] = newWorkplace;
                workplaceUpdatedEvent.newOrChangedWorkplaces.Add(newWorkplace);
            }
        }

        //check for removals
        foreach (var workplaceData in workplaceDatas)
        {
            if (existingMapWorkplaceCoords.Contains(workplaceData.Key))
                continue;

            workplaceUpdatedEvent.removedWorkplaces.Add(workplaceData.Value);
        }

        if (workplaceUpdatedEvent.HasAnythingUpdated)
        {
            InjectionManager.Get<EventDispatcher>().Dispatch(workplaceUpdatedEvent);
        }
    }

    public bool TryAssignResidentToWorkplace(WorkplaceState workplaceState)
    {
        ResidentManager residentManager = InjectionManager.Get<ResidentManager>();
        if (!residentManager.TryGetFirstNotBusyResident(out var chosenResident))
            return false;

        if (!workplaceState.TryAddWorker(chosenResident))
            return false;
        
        residentWorkplaceMap[chosenResident] = workplaceState; //TODO consider removing duplicate data here? update map from WorkplaceData method?? hmm
        var workplaceUpdatedEvent = new WorkplaceUpdatedEvent();
        workplaceUpdatedEvent.newOrChangedWorkplaces.Add(workplaceState);
        InjectionManager.Get<EventDispatcher>().Dispatch(workplaceUpdatedEvent);
        return true;
    }

    public bool TryRemoveResidentFromWorkplace(WorkplaceState workplaceState)
    {
        if (workplaceState.workerCount == 0)
            return false;

        if (!workplaceState.TryRemoveWorker(out var resident))
            return false;

        residentWorkplaceMap.Remove(resident);
        InjectionManager.Get<EventDispatcher>().Dispatch(new WorkplaceUpdatedEvent());
        return true;
    }

    public bool TryGetWorkplaceForResident(ResidentState resident, out WorkplaceState workplaceState)
    {
        return residentWorkplaceMap.TryGetValue(resident, out workplaceState);
    }

    public bool TryGetWorkplaceAtLocation(Vector2I location, out WorkplaceState workplaceState)
    {
        return workplaceDatas.TryGetValue(location, out workplaceState);
    }
}
