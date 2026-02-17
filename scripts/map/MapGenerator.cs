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
    private float vegetationBushesLowerThreshold = 0.7f;
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
    private Vector2I bushesCoords;
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

    //private BetterTerrain bt;
    private MapController mapController;

    public void Setup()
    {
        //bt = new BetterTerrain(baseMapLayer);
        
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
            {
                var atlasCoords = GenerateCellAtlasCoords(affectedCell);
                
                GD.Print($"[MapGenerator] setting cell {affectedCell} to terrain atlas coords {atlasCoords}");
                baseMapLayer.SetCell(affectedCell, 0, atlasCoords);
            }
        }
        
        //var riverCells = FindRiverCells();
        
        /*StringBuilder sb1 = new("river cells: ");
        foreach (var cell in riverCells)
        {
            sb1.Append(cell + " ");
        }
        GD.Print(sb1.ToString());*/

        mapController ??= InjectionManager.Get<MapController>();
        
        //don't want to update cells that the player can see, but will update all other cells to make sure rivers etc are updated right
        List<Vector2I> allCellsToUpdate = new();
        foreach (var affectedCell in allAffectedCells)
        {
            //if (riverCells.Contains(affectedCell) && mapController.GetCellStatus(affectedCell) == CellStatus.Hidden)
                allCellsToUpdate.Add(affectedCell);
        }

        /*StringBuilder sb2 = new("cells to update: ");
        foreach (var cell in allCellsToUpdate)
        {
            sb2.Append(cell + " ");
        }
        GD.Print(sb2.ToString());*/
        
        //bt.UpdateTerrainCells(new Godot.Collections.Array<Vector2I>(allCellsToUpdate), false);
        baseMapLayer.EmitSignal("changed");
    }

    public Vector2I GetDefaultGeneratedAtlasCoordsAtCell(Vector2I cell)
    {
        return GenerateCellAtlasCoords(cell);
    }

    private Vector2I GenerateCellAtlasCoords(Vector2I cell)
    {
        var height = GetPixelSample(heightNoiseImage, cell);
        var rockiness = GetPixelSample(rockyNoiseImage, cell);
                
        if (height < heightWaterUpperThreshold)
            return SetWater(cell, rockiness);
        if (height > heightMountainLowerThreshold)
            return mountainCoords;
        
        var vegetation = GetPixelSample(vegetationNoiseImage, cell);
        
        if (height > heightHillLowerThreshold)
            return SetHill(cell, vegetation);
        
        var sandiness = GetPixelSample(sandyNoiseImage, cell);
        return SetFlat(cell, rockiness, sandiness, vegetation);
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

    private Vector2I SetWater(Vector2I cell, float rockiness)
    {
        //TODO restore after fixing rivers?
        //bt.SetCell(cell, oceanTerrain);
        return rockiness < rockyUpperThreshold ? waterRocksCoords : waterCoords;
    }

    private Vector2I SetHill(Vector2I cell, float vegetation)
    {
        if (vegetation < vegetationGrassLowerThreshold)
            return hillRockCoords;
        
        return vegetation > vegetationTreesLowerThreshold ? hillGrassCoords : hillForestCoords;
    }

    private Vector2I SetFlat(Vector2I cell, float rockiness, float sandiness, float vegetation)
    {
        if (sandiness < sandyUpperThreshold)
            return SetSand(cell, rockiness, vegetation);
            
        if (vegetation < vegetationGrassLowerThreshold)
            return rockiness < rockyUpperThreshold ? stoneCoords : dirtCoords;
        if (vegetation > vegetationTreesLowerThreshold)
            return forestCoords;
        if (vegetation > vegetationBushesLowerThreshold)
            return bushesCoords;
        
        //bt.SetCell(cell, grassTerrain);
        return grassCoords;
    }

    private Vector2I SetSand(Vector2I cell, float rockiness, float vegetation)
    {
        //bt.SetCell(cell, sandTerrain);
        
        if (vegetation > vegetationTreesLowerThreshold)
            return sandPlantsCoords;
        
        return rockiness < rockyUpperThreshold ? sandRocksCoords : sandCoords;
    }
    
    /*private List<Vector2I> FindRiverCells()
    {
        var usedCells = baseMapLayer.GetUsedCells();

        var waterCells = bt.GetTilesInTerrain(oceanTerrain);
        
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
                
                if (waterCells.Contains(baseMapLayer.GetCellTileData(neighbourBefore))
                    || waterCells.Contains(baseMapLayer.GetCellTileData(neighbourAfter)))
                    isRiver = false;
            }

            if (waterNeighbourCount == 0)
                continue;

            if (isRiver)
                riverCells.Add(usedCell); //don't set it here so this cell's water data value can be used to check if other cells are rivers
        }
        
        var terrainRiverCells = bt.GetTilesInTerrain(riverTerrain);

        foreach (var riverCell in riverCells)
        {
            //if already a river, continue
            if (terrainRiverCells.Contains(baseMapLayer.GetCellTileData(riverCell)))
                continue;
            
            //bt.SetCell(riverCell, riverTerrain);
        }

        return riverCells;
    }*/
}
