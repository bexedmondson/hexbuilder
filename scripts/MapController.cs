using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class MapController : Node2D, IInjectable
{
    [Export]
    private TileMapLayer baseMapLayer;
    public TileMapLayer BaseMapLayer => baseMapLayer; 

    [Export]
    private TileMapLayer lockedOverlayLayer;

    private Dictionary<Vector2I, bool> visibleCellUnlockStates = new();

    [Export]
    private MapGenerator mapGenerator;

    private int defaultLockedTileSourceIndex = 0;
    private Vector2I defaultLockedTileAtlasCoords = Vector2I.Zero;

    private MapHighlightController highlightController;

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);
    }

    public override void _Ready() //as this is a child of GameController, this will be called BEFORE GameController tries starting a new game or whatever, so setup is safe to do here
    {
        base._Ready();
        var tileDatabase = InjectionManager.Get<TileDatabase>();
        tileDatabase.AddTileSetTileData(baseMapLayer.TileSet);
        mapGenerator.Setup();
    }

    public void OnNewGame()
    {
        baseMapLayer.Clear();
        lockedOverlayLayer.Clear();
        
        visibleCellUnlockStates.Clear();
        visibleCellUnlockStates[Vector2I.Zero] = true;
        
        mapGenerator.SubscribeIsReady(SetupInitialMap);
    }

    private void SetupInitialMap()
    {
        UnlockCell(Vector2I.Zero);
    }

    public void UnlockCell(Vector2I cell)
    {
        mapGenerator.OnCellUnlocked(cell);
        
        //updating surrounding cells after the above, so that in the above call, relevant tiles can be updated by checking GetCellStatus == Hidden
        //before they're made visible
        visibleCellUnlockStates[cell] = true;
        lockedOverlayLayer.EraseCell(cell);
        
        foreach (var surroundingCell in baseMapLayer.GetSurroundingCells(cell))
        {
            if (visibleCellUnlockStates.ContainsKey(surroundingCell))
                continue;
            
            visibleCellUnlockStates[surroundingCell] = false;
            lockedOverlayLayer.SetCell(surroundingCell, lockedOverlayLayer.TileSet.GetSourceId(defaultLockedTileSourceIndex), defaultLockedTileAtlasCoords);
        }
    }

    public void SetCell(Vector2I cell, TileDatabase.TileInfo tileToSet)
    {
        baseMapLayer.SetCell(cell, baseMapLayer.TileSet.GetSourceId(tileToSet.sourceId), tileToSet.tileCoords);
    }

    public Vector2I GetCellUnderMouse()
    {
        return baseMapLayer.LocalToMap(baseMapLayer.GetLocalMousePosition());
    }

    public CellStatus GetCellStatus(Vector2I cell)
    {
        if (visibleCellUnlockStates.ContainsKey(cell))
        {
            return visibleCellUnlockStates[cell] ? CellStatus.Unlocked : CellStatus.Locked;
        }

        return CellStatus.Hidden;
    }

    public Vector2I[] GetVisibleCells()
    {
        return visibleCellUnlockStates.Keys.ToArray();
    }

    public CurrencySum GetCellUnlockCost(Vector2I cell)
    {
        return new CurrencySum() {
            { CurrencyType.Person, 1 }
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