using Godot;

public partial class MapGenerator : Node
{
    [Export]
    public TileMapLayer baseMapLayer;

    [Export]
    public int terrainAtlasSourceIndex;

    [Export]
    public int noiseSampleScale = 10;

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
    private Vector2I hillForestCoords;
    
    [Export]
    private Vector2I mountainCoords;
    
    [ExportGroup("Noise")]
    [Export]
    private NoiseTexture2D heightNoise;
    [Export]
    private NoiseTexture2D rockyNoise;
    [Export]
    private NoiseTexture2D sandyNoise;
    [Export]
    private NoiseTexture2D vegetationNoise;

    public void Setup()
    {
        //get tile data etc and map to the matching tile indices so getting a random tile of type whatever is easier

        //waterNoise = new NoiseTexture2D();
        //waterNoise.Seamless = true;
        //waterNoise.Noise = new FastNoiseLite();

        int seedChange = GD.RandRange(0, 1000);

        (heightNoise.Noise as FastNoiseLite).Seed += seedChange;
        (rockyNoise.Noise as FastNoiseLite).Seed += seedChange;
        (sandyNoise.Noise as FastNoiseLite).Seed += seedChange;
        (vegetationNoise.Noise as FastNoiseLite).Seed += seedChange;
    }

    public void Generate(Vector2I minPoint, Vector2I maxPoint) //testing only at the moment
    {
        if (heightNoise.GetImage() == null)
        {
            heightNoise.Changed += () => Generate(minPoint, maxPoint);
            return;
        }
        if (rockyNoise.GetImage() == null)
        {
            rockyNoise.Changed += () => Generate(minPoint, maxPoint);
            return;
        }
        if (sandyNoise.GetImage() == null)
        {
            sandyNoise.Changed += () => Generate(minPoint, maxPoint);
            return;
        }
        if (vegetationNoise.GetImage() == null)
        {
            vegetationNoise.Changed += () => Generate(minPoint, maxPoint);
            return;
        }

        var heightNoiseImage = heightNoise.GetImage();
        var rockinessNoiseImage = rockyNoise.GetImage();
        var sandinessNoiseImage = sandyNoise.GetImage();
        var vegetationNoiseImage = vegetationNoise.GetImage();

        for (int x = minPoint.X; x < maxPoint.X; x++)
        {
            for (int y = minPoint.Y; y < maxPoint.Y; y++)
            {
                var cell = new Vector2I(x, y);
                
                var height = GetPixelSample(heightNoiseImage, cell);//.GetPixel(x * noiseSampleScale % heightNoiseImage.GetWidth(), y * noiseSampleScale % heightNoiseImage.GetHeight()).R);
                var rockiness = GetPixelSample(rockinessNoiseImage, cell);// rockinessNoiseImage.GetPixel(x * noiseSampleScale % rockinessNoiseImage.GetWidth(), y * noiseSampleScale % rockinessNoiseImage.GetHeight()).R;
                var sandiness = GetPixelSample(sandinessNoiseImage, cell);//;sandinessNoiseImage.GetPixel(x * noiseSampleScale % sandinessNoiseImage.GetWidth(), y * noiseSampleScale % sandinessNoiseImage.GetHeight()).R);
                var vegetation = GetPixelSample(vegetationNoiseImage, cell);//vegetationNoiseImage.GetPixel(x * noiseSampleScale % vegetationNoiseImage.GetWidth(), y * noiseSampleScale % vegetationNoiseImage.GetHeight()).R);
                
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
        baseMapLayer.SetCell(cell, 0, rockiness < rockyUpperThreshold ? waterRocksCoords : waterCoords);
    }

    private void SetHill(Vector2I cell, float rockiness, float sandiness, float vegetation)
    {
        baseMapLayer.SetCell(cell, 0, vegetation > vegetationTreesLowerThreshold ? hillRockCoords : hillForestCoords);
        //TODO make a new grass hill tile and incorporate here
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
        else
            baseMapLayer.SetCell(cell, 0, vegetation < vegetationTreesLowerThreshold ? grassCoords : forestCoords);
    }

    private void SetSand(Vector2I cell, float rockiness, float vegetation)
    {
        if (vegetation > vegetationTreesLowerThreshold)
            baseMapLayer.SetCell(cell, 0, sandPlantsCoords);
        else //TODO set plain sand with terrain so islands generate
            baseMapLayer.SetCell(cell, 0, rockiness < rockyUpperThreshold ? sandRocksCoords : sandCoords);
    }
}
