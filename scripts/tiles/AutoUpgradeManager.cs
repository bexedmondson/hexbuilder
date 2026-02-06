using Godot;

public class AutoUpgradeManager : IInjectable
{
    private MapController mapController;
    private TimedJobManager timedJobManager;
    private TileDatabase tileDatabase;
    
    public AutoUpgradeManager(MapController mapController)
    {
        this.mapController = mapController;
        InjectionManager.Register(this);
        InjectionManager.Get<EventDispatcher>().Add<NextTurnEvent>(OnNextTurn);
    }
    
    ~AutoUpgradeManager()
    {
        InjectionManager.Deregister(this);
    }

    private void OnNextTurn(NextTurnEvent e)
    {
        timedJobManager ??= InjectionManager.Get<TimedJobManager>();
        tileDatabase ??= InjectionManager.Get<TileDatabase>();
        var usedCells = mapController.BaseMapLayer.GetUsedCells();

        foreach (var usedCell in usedCells)
        {
            if (mapController.GetCellStatus(usedCell) != CellStatus.Unlocked)
                continue;
            
            var cellData = mapController.BaseMapLayer.GetCellCustomData(usedCell);
            
            if (!cellData.TryGetComponent(out AutoUpgradeComponent autoUpgradeComponent))
                continue;

            //already unlocking, continue
            if (timedJobManager.TryGetTimedJobAt(usedCell, out _))
                continue;

            var workplaceManager = InjectionManager.Get<WorkplaceManager>();
            if (workplaceManager.TryGetWorkplaceAtLocation(usedCell, out var workplaceState) && workplaceState.workerCount == 0)
                continue; //TODO figure out how to use WorkplaceHasWorkersRequirement for this instead

            if (!autoUpgradeComponent.CanStartUpgrade(usedCell))
                continue;
            GD.PushWarning($"AutoUpgradeComponent: Starting upgrade at {usedCell} to {autoUpgradeComponent.afterUpgradeTile.GetFileName()}");

            var autoUpgradeJob = new AutoUpgradeTimedJobState(usedCell, autoUpgradeComponent.upgradeDuration, 0);

            var tileInfo = tileDatabase.GetTileInfoForCustomTileData(autoUpgradeComponent.afterUpgradeTile);
            autoUpgradeJob.SetCompleteCallback(() =>
            {
                GD.PushWarning($"AutoUpgradeComponent: Finishing upgrade at {usedCell} to {autoUpgradeComponent.afterUpgradeTile.GetFileName()}");
                mapController.BuildTileAtCell(usedCell, tileInfo);
            });
            
            timedJobManager.AddNewTimedJob(autoUpgradeJob);
        }
    }
}
