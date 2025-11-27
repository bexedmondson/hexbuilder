using System.Collections.Generic;
using Godot;

public partial class MapController : Node2D, IInjectable
{
    [Export]
    private TileMapLayer baseMapLayer;
    public TileMapLayer BaseMapLayer => baseMapLayer; 

    [Export]
    private TileMapLayer lockedOverlayLayer;

    [Export]
    private MapGenerator mapGenerator;

    [ExportCategory("Default Setup")]
    [Export]
    private int defaultCentreTileSourceIndex;
    
    [Export]
    private Vector2I defaultCentreTileCoords;
    
    [Export]
    private int defaultResidentCount;

    private int defaultLockedTileSourceIndex = 0;
    private Vector2I defaultLockedTileAtlasCoords = Vector2I.Zero;

    private MapHighlightController highlightController;
    private MapCurrencyChangeAnalyser currencyChangeAnalyser;
    private ResidentManager residentManager;
    private HousingManager housingManager;
    private WorkplaceManager workplaceManager;
    private EventDispatcher eventDispatcher;
    private CellStatusManager cellStatusManager;

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
        housingManager = new HousingManager(this);
        workplaceManager = new WorkplaceManager(this);
        residentManager = new ResidentManager(this);
        
        var tileDatabase = InjectionManager.Get<TileDatabase>();
        tileDatabase ??= new TileDatabase();
        tileDatabase.AddTileSetTileData(baseMapLayer.TileSet);
        mapGenerator.Setup();
        
    }

    public void OnNewGame()
    {
        baseMapLayer.Clear();
        lockedOverlayLayer.Clear();

        cellStatusManager.OnNewGame();
        mapGenerator.SubscribeIsReady(SetupInitialMap);
    }

    private void SetupInitialMap()
    {
        UnlockCell(Vector2I.Zero);
        
        baseMapLayer.SetCell(Vector2I.Zero, defaultCentreTileSourceIndex, defaultCentreTileCoords);
        
        housingManager.OnNewGame();
        workplaceManager.OnNewGame();
        residentManager.OnNewGame();

        residentManager.CreateResident();
    }

    public void OnCellUnlockInitiated()
    {
        
    }

    public void UnlockCell(Vector2I cell)
    {
        mapGenerator.OnCellUnlocked(cell);

        lockedOverlayLayer.EraseCell(cell);
        
        List<Vector2I> newlyShownCells = cellStatusManager.OnCellUnlocked(cell, baseMapLayer.GetSurroundingCells(cell));

        foreach (var newlyShownCell in newlyShownCells)
        {
            lockedOverlayLayer.SetCell(newlyShownCell, lockedOverlayLayer.TileSet.GetSourceId(defaultLockedTileSourceIndex), defaultLockedTileAtlasCoords);
        }
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public void SetCell(Vector2I cell, TileDatabase.TileInfo tileToSet)
    {
        baseMapLayer.SetCell(cell, baseMapLayer.TileSet.GetSourceId(tileToSet.sourceId), tileToSet.tileCoords);
        
        eventDispatcher.Dispatch(new MapUpdatedEvent());
    }

    public Vector2I GetCellUnderMouse()
    {
        return baseMapLayer.LocalToMap(baseMapLayer.GetLocalMousePosition());
    }

    public CellStatus GetCellStatus(Vector2I cell)
    {
        return cellStatusManager.GetCellStatus(cell);
    }

    public Vector2I[] GetVisibleCells()
    {
        return cellStatusManager.GetVisibleCells();
    }

    public CurrencySum GetCellUnlockCost(Vector2I cell)
    {
        //TODO change
        return new CurrencySum() {
            { CurrencyType.Stone, 1 }
        };
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