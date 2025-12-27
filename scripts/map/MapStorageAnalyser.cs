public class MapStorageAnalyser : IInjectable
{
    private MapController mapController;
    
    public MapStorageAnalyser(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
    }

    public CurrencySum GetTotalStorage()
    {
        CurrencySum total = new();
        foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                continue;

            var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);

            if (cellTileData == null 
                || !cellTileData.TryGetComponent(out StorageCapacityDataComponent storageCapacityComponent)
                || storageCapacityComponent.capacities == null)
                continue;

            total.Add(storageCapacityComponent.capacities);
        }

        return total;
    }
}
