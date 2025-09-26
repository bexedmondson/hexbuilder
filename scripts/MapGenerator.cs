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
    [Export]
    private NoiseTexture2D rockyNoise;
    [Export]
    private NoiseTexture2D sandyNoise;
    [Export]
    private NoiseTexture2D vegetationNoise;

    private BetterTerrain bt;

    public void Setup()
    {
        bt = new BetterTerrain(baseMapLayer);
        
        int seedChange = GD.RandRange(0, 1000);

        (heightNoise.Noise as FastNoiseLite).Seed += seedChange;
        (rockyNoise.Noise as FastNoiseLite).Seed += seedChange;
        (sandyNoise.Noise as FastNoiseLite).Seed += seedChange;
        (vegetationNoise.Noise as FastNoiseLite).Seed += seedChange;
    }

    public void Generate(Rect2I area) //testing only at the moment
    {
        if (heightNoise.GetImage() == null)
        {
            heightNoise.Changed += () => Generate(area);
            return;
        }
        if (rockyNoise.GetImage() == null)
        {
            rockyNoise.Changed += () => Generate(area);
            return;
        }
        if (sandyNoise.GetImage() == null)
        {
            sandyNoise.Changed += () => Generate(area);
            return;
        }
        if (vegetationNoise.GetImage() == null)
        {
            vegetationNoise.Changed += () => Generate(area);
            return;
        }

        var heightNoiseImage = heightNoise.GetImage();
        var rockinessNoiseImage = rockyNoise.GetImage();
        var sandinessNoiseImage = sandyNoise.GetImage();
        var vegetationNoiseImage = vegetationNoise.GetImage();

        for (int x = area.Position.X; x < area.End.X; x++)
        {
            for (int y = area.Position.Y; y < area.End.Y; y++)
            {
                var cell = new Vector2I(x, y);
                
                var height = GetPixelSample(heightNoiseImage, cell);
                var rockiness = GetPixelSample(rockinessNoiseImage, cell);
                var sandiness = GetPixelSample(sandinessNoiseImage, cell);
                var vegetation = GetPixelSample(vegetationNoiseImage, cell);
                
                if (height < heightWaterUpperThreshold)
                    SetWater(cell, rockiness, sandiness, vegetation);
                else if (height > heightMountainLowerThreshold)
                    baseMapLayer.SetCell(new Vector2I(x, y), 0, mountainCoords);
                else if (height > heightHillLowerThreshold)
                    SetHill(cell, rockiness, sandiness, vegetation);
                else
                    SetFlat(cell, rockiness, sandiness, vegetation);
            }
        }

        FindRiverTiles();
        
        bt.UpdateTerrainArea(area);
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

    private void SetWater(Vector2I cell, float rockiness, float sandiness, float vegetation)
    {
        bt.SetCell(cell, oceanTerrain);
        baseMapLayer.SetCell(cell, 0, rockiness < rockyUpperThreshold ? waterRocksCoords : waterCoords);
    }

    private void SetHill(Vector2I cell, float rockiness, float sandiness, float vegetation)
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
        //2 = sand terrain, setup in editor. maybe should move this to an export or get it dynamically? this works for now though
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
