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
        bool workplacesChanged = false;
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
                
                workplacesChanged = true;

            }
            else
            {
                //TODO pass in CustomTileData instead?
                workplaceDatas[cell] = new WorkplaceData(cell, cellData.workerCapacity, cellData.GetFileName(), tileDatabase.GetTileTexture(cellData));
                workplacesChanged = true;
            }
        }

        foreach (var workplaceData in workplaceDatas)
        {
            if (existingMapWorkplaceCoords.Contains(workplaceData.Key))
                continue;

            workplacesChanged = true;
        }

        if (workplacesChanged)
        {
            InjectionManager.Get<EventDispatcher>().Dispatch(new WorkplaceUpdatedEvent());
        }
    }

    public bool TryAssignResidentToWorkplace(ResidentData resident)
    {
        List<WorkplaceData> availableWorkplaces = new();
        foreach (var workplaceData in AllWorkplaceDatas)
        {
            if (workplaceData.workers.Length < workplaceData.capacity)
                availableWorkplaces.Add(workplaceData);
        }

        if (availableWorkplaces.Count == 0)
            return false;

        var chosenWorkplace = availableWorkplaces[GD.RandRange(0, availableWorkplaces.Count - 1)];
        chosenWorkplace.TryAddWorker(resident);
        
        residentWorkplaceMap[resident] = chosenWorkplace; //TODO consider removing duplicate data here? update map from WorkplaceData method?? hmm
        return true;
    }

    public bool TryGetWorkplaceForResident(ResidentData resident, out WorkplaceData workplaceData)
    {
        return residentWorkplaceMap.TryGetValue(resident, out workplaceData);
    }
}
