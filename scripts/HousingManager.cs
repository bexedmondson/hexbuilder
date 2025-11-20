
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HousingManager : IInjectable
{
    private MapController mapController;
    private TileDatabase tileDatabase;

    private Dictionary<Vector2I, HouseData> houseDatas = new();
    public HouseData[] AllHouseDatas => houseDatas.Values.ToArray();
    
    public HousingManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public void OnNewGame()
    {
        tileDatabase = InjectionManager.Get<TileDatabase>();

        InjectionManager.Get<EventDispatcher>().Add<MapUpdatedEvent>(OnMapUpdated);
    }

    private void OnMapUpdated(MapUpdatedEvent _)
    {
        var usedCells = mapController.BaseMapLayer.GetUsedCells();
        
        List<Vector2I> existingMapHouseCoords = new();
        
        foreach (var cell in usedCells)
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;
            
            var cellData = mapController.BaseMapLayer.GetCellCustomData(cell);
            
            if (cellData.residentCapacity == 0)
                continue;
            
            existingMapHouseCoords.Add(cell);

            if (houseDatas.ContainsKey(cell))
                continue;
            
            houseDatas[cell] = new HouseData(cell, cellData.residentCapacity);
        }

        foreach (var houseData in houseDatas)
        {
            if (existingMapHouseCoords.Contains(houseData.Key))
                continue;
            
            //TODO clean up here - move residents?
        }
    }
}
