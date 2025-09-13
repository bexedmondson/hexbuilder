using System.Collections.Generic;
using Godot;

public class TileDatabase : IInjectable
{
    public record TileInfo
    {
        public TileSet tileSet;
        public int sourceId;
        public Vector2I tileCoords;
        public AtlasTexture tileTexture;
        public CustomTileData tileData;
    }
    
    private List<TileInfo> tileInfos = new();
    
    public TileDatabase()
    {
        InjectionManager.Register(this);
    }

    public void AddTileSetTileData(TileSet tileSet)
    {
        for (int i = 0; i < tileSet.GetSourceCount(); i++)
        {
            int sourceId = tileSet.GetSourceId(i);
            if (!tileSet.HasSource(sourceId))
                continue;
            var source = tileSet.GetSource(sourceId) as TileSetAtlasSource; //all sources will be atlases so assumption is acceptable
            for (int j = 0; j < source.GetTilesCount(); j++)
            {
                TileInfo tileInfo = new TileInfo();
                tileInfo.tileSet = tileSet;
                tileInfo.sourceId = sourceId;
                
                tileInfo.tileCoords = source.GetTileId(j);
                tileInfo.tileTexture = new AtlasTexture();
                tileInfo.tileTexture.Atlas = source.Texture;
                tileInfo.tileTexture.Region = source.GetTileTextureRegion(tileInfo.tileCoords);
                
                tileInfo.tileData = source.GetTileData(tileInfo.tileCoords, 0).GetCustomData("data").Obj as CustomTileData;
                
                tileInfos.Add(tileInfo);
            }
        }
    }

    public List<TileInfo> GetAllCompatibleTileInfos(CustomTileData customTileData)
    {
        List<TileInfo> compatibleTileInfos = new();
        foreach (var tileInfo in tileInfos)
        {
            if (tileInfo.tileData?.canBePlacedOn?.Contains(customTileData) == true)
                compatibleTileInfos.Add(tileInfo);
        }
        return compatibleTileInfos;
    }
}