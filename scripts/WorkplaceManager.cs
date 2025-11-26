using System.Collections.Generic;
using System.Linq;
using Godot;

public class WorkplaceManager : IInjectable
{
    private MapController mapController;
    private TileDatabase tileDatabase;

    private Dictionary<Vector2I, WorkplaceData> workplaceDatas = new();
    public WorkplaceData[] AllWorkplaceDatas => workplaceDatas.Values.ToArray();

    private Dictionary<ResidentData, WorkplaceData> residentWorkplaceMap = new();
    
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
                if (existingWorkplaceData.capacity == cellData.workerCapacity)
                    continue;

                existingWorkplaceData.ChangeCapacity(cellData.workerCapacity, out List<ResidentData> removedWorkers);

                foreach (var removedWorker in removedWorkers)
                {
                    residentWorkplaceMap.Remove(removedWorker);
                }
                
                workplaceUpdatedEvent.newOrChangedWorkplaces.Add(existingWorkplaceData);
            }
            else
            {
                //TODO pass in CustomTileData instead?
                var newWorkplace = new WorkplaceData(cell, cellData.workerCapacity, cellData.GetFileName(), tileDatabase.GetTileTexture(cellData));
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

    public bool TryAssignResidentToWorkplace(WorkplaceData workplaceData)
    {
        ResidentManager residentManager = InjectionManager.Get<ResidentManager>();
        if (!residentManager.TryGetFirstResidentWithoutWorkplace(out var chosenResident))
            return false;

        if (!workplaceData.TryAddWorker(chosenResident))
            return false;
        
        residentWorkplaceMap[chosenResident] = workplaceData; //TODO consider removing duplicate data here? update map from WorkplaceData method?? hmm
        var workplaceUpdatedEvent = new WorkplaceUpdatedEvent();
        workplaceUpdatedEvent.newOrChangedWorkplaces.Add(workplaceData);
        InjectionManager.Get<EventDispatcher>().Dispatch(workplaceUpdatedEvent);
        return true;
    }

    public bool TryRemoveResidentFromWorkplace(WorkplaceData workplaceData)
    {
        if (workplaceData.workerCount == 0)
            return false;

        if (!workplaceData.TryRemoveWorker(out var resident))
            return false;

        residentWorkplaceMap.Remove(resident);
        InjectionManager.Get<EventDispatcher>().Dispatch(new WorkplaceUpdatedEvent());
        return true;
    }

    public bool TryGetWorkplaceForResident(ResidentData resident, out WorkplaceData workplaceData)
    {
        return residentWorkplaceMap.TryGetValue(resident, out workplaceData);
    }

    public bool TryGetWorkplaceAtLocation(Vector2I location, out WorkplaceData workplaceData)
    {
        return workplaceDatas.TryGetValue(location, out workplaceData);
    }
}
