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

    private int defaultLockedTileSourceIndex = 0;
    private Vector2I defaultLockedTileAtlasCoords = Vector2I.Zero;
    private int defaultUnlockedTileSourceIndex = 0;
    private Vector2I defaultUnlockedTileAtlasCoords = Vector2I.Right * 3;

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
    }

    public void OnNewGame()
    {
        baseMapLayer.Clear();
        lockedOverlayLayer.Clear();
        
        visibleCellUnlockStates.Clear();
        visibleCellUnlockStates[Vector2I.Zero] = true;

        var defaultUnlockedTileSource = baseMapLayer.TileSet.GetSource(defaultUnlockedTileSourceIndex) as TileSetAtlasSource;
        
        baseMapLayer.SetCell(Vector2I.Zero, baseMapLayer.TileSet.GetSourceId(defaultUnlockedTileSourceIndex), defaultUnlockedTileAtlasCoords);
        
        foreach (var surroundingCell in baseMapLayer.GetSurroundingCells(Vector2I.Zero))
        {
            visibleCellUnlockStates[surroundingCell] = false;
            baseMapLayer.SetCell(surroundingCell, baseMapLayer.TileSet.GetSourceId(defaultUnlockedTileSourceIndex), GetRandomTile(defaultUnlockedTileSource));
            lockedOverlayLayer.SetCell(surroundingCell, lockedOverlayLayer.TileSet.GetSourceId(defaultLockedTileSourceIndex), defaultLockedTileAtlasCoords);
        }
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

    public Dictionary<CurrencyType, int> GetCellUnlockCost(Vector2I cell)
    {
        return new Dictionary<CurrencyType, int>() {
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
}