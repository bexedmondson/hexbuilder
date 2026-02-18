using System.Collections.Generic;
using Godot;

public partial class MapController : Node2D, IInjectable
{
    [Export]
    private BaseTileMapLayer baseMapLayer;
    public BaseTileMapLayer BaseMapLayer => baseMapLayer;

    [Export]
    private MapGenerator mapGenerator;

    [ExportCategory("Default Setup")]
    [Export]
    private int plainGrassTileSourceIndex;
    
    [Export]
    private Vector2I plainGrassTileCoords;
    
    [Export]
    private int defaultResidentCount;

    private int defaultLockedTileSourceIndex = 0;
    private Vector2I defaultLockedTileAtlasCoords = Vector2I.Zero;

    private TileDatabase tileDatabase;

    private MapHighlightController highlightController;
    private MapCurrencyChangeAnalyser currencyChangeAnalyser;
    private MapStorageAnalyser storageAnalyser;
    private ResidentManager residentManager;
    private HousingManager housingManager;
    private WorkplaceManager workplaceManager;
    private TimedTaskManager timedTaskManager;
    private EventDispatcher eventDispatcher;
    private CellStatusManager cellStatusManager;
    private AutoUpgradeManager autoUpgradeManager;
    
    private CellBuildStatsTracker cellBuildStatsTracker;

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);
    }

    public override void _Ready() //as this is a child of GameController, this will be called BEFORE GameController tries starting a new game or whatever, so setup is safe to do here
    {
        base._Ready();
        eventDispatcher = InjectionManager.Get<EventDispatcher>();
        
        cellStatusManager = new CellStatusManager();
        currencyChangeAnalyser = new MapCurrencyChangeAnalyser(this);
        storageAnalyser = new MapStorageAnalyser(this);
        housingManager = new HousingManager(this);
        workplaceManager = new WorkplaceManager(this);
        timedTaskManager = new TimedTaskManager(this);
        residentManager = new ResidentManager(this);
        cellBuildStatsTracker = new CellBuildStatsTracker();
        autoUpgradeManager = new AutoUpgradeManager(this);
        
        tileDatabase = InjectionManager.Get<TileDatabase>();
        tileDatabase ??= new TileDatabase();
        tileDatabase.AddTileSetTileData(baseMapLayer.TileSet);
        mapGenerator.Setup();
    }

    public void OnNewGame()
    {
        baseMapLayer.Clear();

        cellStatusManager.OnNewGame();
        mapGenerator.SubscribeIsReady(SetupInitialMap);
    }

    private void SetupInitialMap()
    {
        var startPattern = baseMapLayer.TileSet.GetPattern(0);
        baseMapLayer.SetPattern(Vector2I.Zero, startPattern);
        
        var usedCells = startPattern.GetUsedCells();
        foreach (var startCell in usedCells)
        {
            UnlockCell(startCell);
        }
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
        
        housingManager.OnNewGame();
        workplaceManager.OnNewGame();
        residentManager.OnNewGame();

        residentManager.CreateResident();
    }

    public void OnCellUnlockInitiated(Vector2I cellToStartUnlock)
    {
        if (residentManager.GetNotBusyResidentCount() == 0)
            return;

        //already doing something here, don't try to start anything new
        if (timedTaskManager.TryGetTimedTaskAt(cellToStartUnlock, out _))
            return;

        //can't start the unlock if the cell isn't both visible and locked!
        if (cellStatusManager.GetCellStatus(cellToStartUnlock) != CellStatus.Locked)
            return;

        int unlockTurnCount = 1;
        var cellCustomData = baseMapLayer.GetCellCustomData(cellToStartUnlock);
        if (cellCustomData != null && cellCustomData.TryGetComponent(out TerrainUnlockTimeComponent unlockTimeComponent))
            unlockTurnCount = unlockTimeComponent.turnCount;

        var workerCountRequirement = GetCellUnlockWorkerRequirement(cellToStartUnlock);
        
        var cellUnlockTimedTaskData = new CellUnlockTimedTaskState(cellToStartUnlock, unlockTurnCount, workerCountRequirement);
        
        for (int i = 0; i < workerCountRequirement; i++)
        {
            timedTaskManager.TryAssignResidentToTimedTask(cellUnlockTimedTaskData);
        }
        timedTaskManager.AddNewTimedTask(cellUnlockTimedTaskData);
        
        cellStatusManager.OnCellUnlockInitiated(cellToStartUnlock);
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public void UnlockCell(Vector2I cell)
    {
        mapGenerator.OnCellUnlocked(cell);

        int cellSourceId = baseMapLayer.GetCellSourceId(cell);
        Vector2I cellAtlasCoords = baseMapLayer.GetCellAtlasCoords(cell);
        if (cellSourceId != -1 && cellAtlasCoords != Vector2I.One * -1)
        {
            GD.Print($"[MapGenerator] setting cell {cell} to source ID {cellSourceId}, atlas coords {cellAtlasCoords}");
            baseMapLayer.SetCell(cell, cellSourceId, cellAtlasCoords, 0); //setting to the default (i.e. not greyed out) version of the tile
        }
        
        List<Vector2I> newlyShownCells = cellStatusManager.OnCellUnlocked(cell, baseMapLayer.GetSurroundingCells(cell));

        foreach (var newlyShownCell in newlyShownCells)
        {
            int newlyShownCellSourceId = baseMapLayer.GetCellSourceId(newlyShownCell);
            Vector2I newlyShownCellAtlasCoords = baseMapLayer.GetCellAtlasCoords(newlyShownCell);
            if (baseMapLayer.TileSet.GetSource(newlyShownCellSourceId).HasAlternativeTile(newlyShownCellAtlasCoords, 1))
            {
                GD.Print($"[MapGenerator] setting cell {newlyShownCell} to terrain atlas coords {newlyShownCellAtlasCoords}");
                baseMapLayer.SetCell(newlyShownCell, baseMapLayer.GetCellSourceId(newlyShownCell), baseMapLayer.GetCellAtlasCoords(newlyShownCell), 1); //where 1 is the modulated (darkened) version of the cell
            }
        }
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public CustomTileData GetDefaultGeneratedTileAtCell(Vector2I cell)
    {
        //if in the starting set of tiles, revert to that rather than what would've generated under it
        //and been overwritten by the pattern - UNLESS it's a starting building, in which case just revert to grass
        var startPattern = baseMapLayer.TileSet.GetPattern(0);
        var usedCells = startPattern.GetUsedCells();
        foreach (var startCell in usedCells)
        {
            if (startCell != cell)
                continue;

            if (startPattern.GetCellSourceId(startCell) != plainGrassTileSourceIndex) //i.e. if not a terrain cell
            {
                return tileDatabase.GetTileInfoForAtlasCoords(plainGrassTileSourceIndex, plainGrassTileCoords).tileData;
            }

            var patternAtlasCoords = startPattern.GetCellAtlasCoords(startCell);
            return tileDatabase.GetTileInfoForAtlasCoords(0, patternAtlasCoords).tileData;
        }
        
        var atlasCoords = mapGenerator.GetDefaultGeneratedAtlasCoordsAtCell(cell);
        return tileDatabase.GetTileInfoForAtlasCoords(0, atlasCoords).tileData; //TODO make the source id not just a random hardcoded 0
    }

    public void SetCellBusy(Vector2I cell)
    {
        cellStatusManager.SetCellBusy(cell);
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public void OnTimedTaskEnded(Vector2I cell)
    {
        cellStatusManager.SetCellUnlockedFromBusy(cell);
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public void BuildTileAtCell(Vector2I cell, CustomTileData customTileData)
    {
        var tileInfo = tileDatabase.GetTileInfoForCustomTileData(customTileData);
        BuildTileAtCell(cell, tileInfo);
    }

    public void BuildTileAtCell(Vector2I cell, TileDatabase.TileInfo tileToSet)
    {
        GD.Print($"[MapGenerator] setting cell {cell} to {tileToSet.tileData.GetFileName()}, source id {tileToSet.sourceId}, atlas coords {tileToSet.tileCoords}");
        baseMapLayer.SetCell(cell, baseMapLayer.TileSet.GetSourceId(tileToSet.sourceId), tileToSet.tileCoords);

        cellBuildStatsTracker.OnCellTileBuilt(tileToSet.tileData);
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public Vector2I GetCellUnderMouse(Vector2 screenPosition)
    {
        return baseMapLayer.LocalToMap(baseMapLayer.MakeCanvasPositionLocal(screenPosition));
    }

    public CellStatus GetCellStatus(Vector2I cell)
    {
        return cellStatusManager.GetCellStatus(cell);
    }

    public Vector2I[] GetVisibleCells()
    {
        return cellStatusManager.GetVisibleCells();
    }

    public int GetCellUnlockWorkerRequirement(Vector2I cell)
    {
        return Mathf.Max(1, Mathf.FloorToInt(GetDistanceFromCentreTo(cell) / 2f));
    }

    private int GetDistanceFromCentreTo(Vector2I cell)
    {
        var x = Mathf.Abs(cell.X);
        
        if (cell.X == cell.Y) //accounting for hex coords
            return x * 2;
        
        var y = Mathf.Abs(cell.Y);
        return x > y ? x : y;
    }

    public Dictionary<Vector2I, int> GetSurroundingCellsUpToDistance(Vector2I centreCell, int maxDistance)
    {
        Dictionary<Vector2I, int> neighbourDistanceDict = new(){
            { centreCell, 0 }
        };

        List<Vector2I> seenCells = new() { centreCell };
        List<Vector2I> newCellsFound = new();

        for (int dist = 0; dist < maxDistance; dist++)
        {
            newCellsFound.Clear();
            for (int i = 0; i < seenCells.Count; i++)
            {
                var seenCell = seenCells[i];
                foreach (var neighbour in baseMapLayer.GetSurroundingCells(seenCell))
                {
                    //only make a change if this cell isn't already in the dict
                    bool added = neighbourDistanceDict.TryAdd(neighbour, dist + 1);
                    
                    if (added)
                        newCellsFound.Add(neighbour);
                }
            }

            seenCells.Clear();
            seenCells.AddRange(newCellsFound);
        }
        
        return neighbourDistanceDict;
    }

    public void HighlightNeighbourEffects(Vector2I cell, TileDatabase.TileInfo selectedTileInfo)
    {
        //show effects on immediate neighbours, for now (TODO: should check for effects on further away tiles also)

        highlightController ??= InjectionManager.Get<MapHighlightController>();

        highlightController.ClearAllExcept(cell);

        var neighbours = baseMapLayer.GetSurroundingCells(cell);

        foreach (var neighbour in neighbours)
        {
            if (GetCellStatus(neighbour) != CellStatus.Unlocked)
                continue;
            
            var neighbourTileData = baseMapLayer.GetCellCustomData(neighbour);
            if (neighbourTileData?.adjacencies == null)
                continue;
            
            foreach (var adjacency in neighbourTileData.adjacencies)
            {
                if (adjacency.requiredTile != selectedTileInfo.tileData)
                    continue;

                //don't want to do a foreach here but this will do for now, until i find a nice way to show
                //the affected currency with the indicators
                foreach (var kvp in adjacency.currencyEffect)
                {
                    if (kvp.Value > 0)
                        highlightController.OnHighlightBenefitTile(neighbour);
                    else
                        highlightController.OnHighlightDrawbackTile(neighbour);
                    break;
                }
            }
        }
    }
}