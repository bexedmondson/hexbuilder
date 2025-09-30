using System;
using System.Collections.Generic;
using Godot;

public partial class MapGenerator : Node
{
    [Export]
    public TileMapLayer baseMapLayer;

    [Export]
    public int terrainAtlasSourceIndex;

    [Export]
    public int noiseSampleScale = 10;

    [Export]
    private CustomTileData waterCustomTileData;

    [ExportGroup("Thresholds")]
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float rockyUpperThreshold = 0.2f;
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float sandyUpperThreshold = 0.3f;
    
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float vegetationGrassLowerThreshold = 0.3f;
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float vegetationTreesLowerThreshold = 0.8f;
    
    [ExportSubgroup("Height")]
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float heightWaterUpperThreshold = 0.3f;
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float heightHillLowerThreshold = 0.65f;
    [Export(PropertyHint.Range, "0.0,1.0,")]
    private float heightMountainLowerThreshold = 0.8f;

    [ExportGroup("Tile Coords")]
    [ExportSubgroup("Plain")]
    [Export]
    private Vector2I dirtCoords;
    [Export]
    private Vector2I stoneCoords;
    
    [ExportSubgroup("Vegetation")]
    [Export]
    private Vector2I grassCoords;
    [Export]
    private Vector2I forestCoords;
    
    [ExportSubgroup("Water")]
    [Export]
    private Vector2I waterCoords;
    [Export]
    private Vector2I waterRocksCoords;
    
    [ExportSubgroup("Sand")]
    [Export]
    private Vector2I sandCoords;
    [Export]
    private Vector2I sandPlantsCoords;
    [Export]
    private Vector2I sandRocksCoords;

    [ExportSubgroup("Hills")]
    [Export]
    private Vector2I hillRockCoords;

    [Export]
    private Vector2I hillGrassCoords;

    [Export]
    private Vector2I hillForestCoords;
    
    [Export]
    private Vector2I mountainCoords;

    [ExportGroup("Terrain Indices")]
    [Export]
    private int riverTerrain;
    [Export]
    private int grassTerrain;
    [Export]
    private int sandTerrain;
    [Export]
    private int oceanTerrain;
    [Export]
    private int bridgeTerrain;
    
    [ExportGroup("Noise")]
    [Export]
    private NoiseTexture2D heightNoise;
    private Image heightNoiseImage;
    [Export]
    private NoiseTexture2D rockyNoise;
    private Image rockyNoiseImage;
    [Export]
    private NoiseTexture2D sandyNoise;
    private Image sandyNoiseImage;
    [Export]
    private NoiseTexture2D vegetationNoise;
    private Image vegetationNoiseImage;

    private BetterTerrain bt;
    private MapController mapController;

    public void Setup()
    {
        bt = new BetterTerrain(baseMapLayer);
        
        int seedChange = GD.RandRange(0, 1000);

        (heightNoise.Noise as FastNoiseLite).Seed += seedChange;
        (rockyNoise.Noise as FastNoiseLite).Seed += seedChange;
        (sandyNoise.Noise as FastNoiseLite).Seed += seedChange;
        (vegetationNoise.Noise as FastNoiseLite).Seed += seedChange;
    }

    //will call immediately if all noise images are already good to go
    public void SubscribeIsReady(Action onReadyAction)
    {
        //i kinda hate this but it's the most straightforward way to wait until noise images exist
        heightNoiseImage = heightNoise.GetImage();
        if (heightNoiseImage == null)
        {
            heightNoise.Changed += () => SubscribeIsReady(onReadyAction);
            return;
        }
        
        rockyNoiseImage = rockyNoise.GetImage();
        if (rockyNoiseImage == null)
        {
            rockyNoise.Changed += () => SubscribeIsReady(onReadyAction);
            return;
        }

        sandyNoiseImage = sandyNoise.GetImage();
        if (sandyNoiseImage == null)
        {
            sandyNoise.Changed += () => SubscribeIsReady(onReadyAction);
            return;
        }

        vegetationNoiseImage = vegetationNoise.GetImage();
        if (vegetationNoiseImage == null)
        {
            vegetationNoise.Changed += () => SubscribeIsReady(onReadyAction);
            return;
        }
        
        onReadyAction?.Invoke();
    }
    
    public void OnCellUnlocked(Vector2I centreCell) //only call this after calling Setup and coming back from SubscribeIsReady!
    {
        //if centre cell is currently empty, set it with generate function
        //then for each surrounding cell, do the same
        //then for each cell surrounding those, do the same
        //then run the river check
        //then call update terrain cells for all cells in the above list - just to be safe, ensure cell isn't visible before doing this

        HashSet<Vector2I> allAffectedCells = [ centreCell ];
        foreach (var immediateNeighbour in baseMapLayer.GetSurroundingCells(centreCell))
        {
            //updating neighbours-of-neighbours so that later on, neighbours can correctly check if they should be rivers or not
            //don't need to add neighbour itself - will be added because neighbours are also neighbours of each other
            allAffectedCells.UnionWith(baseMapLayer.GetSurroundingCells(immediateNeighbour));
        }

        foreach (var affectedCell in allAffectedCells)
        {
            if (baseMapLayer.IsCellEmpty(affectedCell))
                GenerateCell(affectedCell);
        }
        
        FindRiverTiles();

        mapController ??= InjectionManager.Get<MapController>();
        
        //don't want to update cells that the player can see, but will update all other cells to make sure rivers etc are updated right
        List<Vector2I> allCellsToUpdate = new();
        foreach (var affectedCell in allAffectedCells)
        {
            if (mapController.GetCellStatus(affectedCell) == CellStatus.Hidden)
                allCellsToUpdate.Add(affectedCell);
        }
        
        bt.UpdateTerrainCells(new Godot.Collections.Array<Vector2I>(allCellsToUpdate));
    }

    private void GenerateCell(Vector2I cell)
    {
        var height = GetPixelSample(heightNoiseImage, cell);
        var rockiness = GetPixelSample(rockyNoiseImage, cell);
                
        if (height < heightWaterUpperThreshold)
        {
            SetWater(cell, rockiness);
            return;
        }
        if (height > heightMountainLowerThreshold)
        {
            baseMapLayer.SetCell(cell, 0, mountainCoords);
            return;
        }
        
        var vegetation = GetPixelSample(vegetationNoiseImage, cell);
        
        if (height > heightHillLowerThreshold)
        {
            SetHill(cell, vegetation);
            return;
        }
        
        var sandiness = GetPixelSample(sandyNoiseImage, cell);
        SetFlat(cell, rockiness, sandiness, vegetation);
    }

    private float GetPixelSample(Image image, Vector2I cell)
    {
        var xSample = cell.X * noiseSampleScale % image.GetWidth();
        if (xSample < 0)
            xSample += image.GetWidth();
        var ySample = cell.Y * noiseSampleScale % image.GetHeight();
        if (ySample < 0)
            ySample += image.GetHeight();
        return image.GetPixel(xSample, ySample).R; //r, g, b all identical with noise like this
    }

    private void SetWater(Vector2I cell, float rockiness)
    {
        bt.SetCell(cell, oceanTerrain);
        baseMapLayer.SetCell(cell, 0, rockiness < rockyUpperThreshold ? waterRocksCoords : waterCoords);
    }

    private void SetHill(Vector2I cell, float vegetation)
    {
        if (vegetation < vegetationGrassLowerThreshold)
            baseMapLayer.SetCell(cell, 0, hillRockCoords);
        
        baseMapLayer.SetCell(cell, 0, vegetation > vegetationTreesLowerThreshold ? hillGrassCoords : hillForestCoords);
    }

    private void SetFlat(Vector2I cell, float rockiness, float sandiness, float vegetation)
    {
        if (sandiness < sandyUpperThreshold)
        {
            SetSand(cell, rockiness, vegetation);
            return;
        }
            
        if (vegetation < vegetationGrassLowerThreshold)
            baseMapLayer.SetCell(cell, 0, rockiness < rockyUpperThreshold ? stoneCoords : dirtCoords);
        else if (vegetation > vegetationTreesLowerThreshold)
            baseMapLayer.SetCell(cell, 0, forestCoords);
        else
        {
            bt.SetCell(cell, grassTerrain);
        }
    }

    private void SetSand(Vector2I cell, float rockiness, float vegetation)
    {
        bt.SetCell(cell, sandTerrain);
        
        if (vegetation > vegetationTreesLowerThreshold)
            baseMapLayer.SetCell(cell, 0, sandPlantsCoords);
        else
            baseMapLayer.SetCell(cell, 0, rockiness < rockyUpperThreshold ? sandRocksCoords : sandCoords);
    }
    
    private void FindRiverTiles()
    {
        var usedCells = baseMapLayer.GetUsedCells();

        List<Vector2I> riverCells = new();

        foreach (var usedCell in usedCells)
        {
            if (baseMapLayer.GetCellCustomData(usedCell) != waterCustomTileData)
                continue;
            
            var neighbours = baseMapLayer.GetSurroundingCells(usedCell);
            bool isRiver = true;
            int waterNeighbourCount = 0;
            
            //all neighbouring water tiles should have non-water neighbours either side
            for (int i = 0; i < neighbours.Count; i++)
            {
                var neighbourToCheck = neighbours[i];
                if (baseMapLayer.GetCellCustomData(neighbourToCheck) != waterCustomTileData)
                    continue;

                waterNeighbourCount++;
                
                Vector2I neighbourBefore = i > 0 ? neighbours[i - 1] : neighbours[^1];
                Vector2I neighbourAfter = i < neighbours.Count - 1 ? neighbours[i + 1] : neighbours[0];
                
                if (baseMapLayer.GetCellCustomData(neighbourBefore) == waterCustomTileData
                    || baseMapLayer.GetCellCustomData(neighbourAfter) == waterCustomTileData)
                    isRiver = false;
            }

            if (waterNeighbourCount == 0)
                continue;

            if (isRiver)
                riverCells.Add(usedCell); //don't set it here so this cell's water data value can be used to check if other cells are rivers
        }

        foreach (var riverCell in riverCells)
        {
            bt.SetCell(riverCell, riverTerrain);
        }
    }
}
