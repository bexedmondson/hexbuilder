using System.Collections.Generic;
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
    private int defaultUnlockedTileSourceIndex = 0;
    private Vector2I defaultUnlockedTileAtlasCoords = Vector2I.Right * 3;

    private MapHighlightController highlightController;

    public override void _EnterTree()
    {
        base._EnterTree();
        InjectionManager.Register(this);
    }

    public override void _Ready()
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

        mapGenerator.Generate(Vector2I.One * -10, Vector2I.One * 10); //temporarily, for testing

        var defaultUnlockedTileSource = baseMapLayer.TileSet.GetSource(defaultUnlockedTileSourceIndex) as TileSetAtlasSource;
        
        //baseMapLayer.SetCell(Vector2I.Zero, baseMapLayer.TileSet.GetSourceId(defaultUnlockedTileSourceIndex), defaultUnlockedTileAtlasCoords);
        
        /*foreach (var surroundingCell in baseMapLayer.GetSurroundingCells(Vector2I.Zero))
        {
            visibleCellUnlockStates[surroundingCell] = false;
            baseMapLayer.SetCell(surroundingCell, baseMapLayer.TileSet.GetSourceId(defaultUnlockedTileSourceIndex), GetRandomTile(defaultUnlockedTileSource));
            lockedOverlayLayer.SetCell(surroundingCell, lockedOverlayLayer.TileSet.GetSourceId(defaultLockedTileSourceIndex), defaultLockedTileAtlasCoords);
        }*/
    }

    private Vector2I GetRandomTile(TileSetAtlasSource atlasSource)
    {
        int tileCount = atlasSource.GetTilesCount();
        return atlasSource.GetTileId((int)(GD.Randi() % tileCount));
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

    public CurrencySum GetCellUnlockCost(Vector2I cell)
    {
        return new CurrencySum() {
            { CurrencyType.Person, 1 }
        };
    }

    public void UnlockCell(Vector2I cell)
    {
        visibleCellUnlockStates[cell] = true;
        lockedOverlayLayer.EraseCell(cell);
        
        var defaultUnlockedTileSource = baseMapLayer.TileSet.GetSource(defaultUnlockedTileSourceIndex) as TileSetAtlasSource;
        
        foreach (var surroundingCell in baseMapLayer.GetSurroundingCells(cell))
        {
            if (visibleCellUnlockStates.ContainsKey(surroundingCell))
                continue;
            
            visibleCellUnlockStates[surroundingCell] = false;
            baseMapLayer.SetCell(surroundingCell, baseMapLayer.TileSet.GetSourceId(defaultUnlockedTileSourceIndex), GetRandomTile(defaultUnlockedTileSource));
            lockedOverlayLayer.SetCell(surroundingCell, lockedOverlayLayer.TileSet.GetSourceId(defaultLockedTileSourceIndex), defaultLockedTileAtlasCoords);
        }
    }

    public void SetCell(Vector2I cell, TileDatabase.TileInfo tileToSet)
    {
        baseMapLayer.SetCell(cell, baseMapLayer.TileSet.GetSourceId(tileToSet.sourceId), tileToSet.tileCoords);
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