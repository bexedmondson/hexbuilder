using System.Collections.Generic;
using Godot;

[Tool]
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

    public List<TileInfo> AllTileInfos => new(tileInfos);

    private List<TileInfo> allBuildingTileInfos = null;
    public List<TileInfo> AllBuildingTileInfos
    {
        get
        {
            if (allBuildingTileInfos != null)
                return allBuildingTileInfos;

            allBuildingTileInfos = new();
            foreach (var tileInfo in tileInfos)
            {
                if (tileInfo.sourceId == 1) //there has to be a better way to do this. no API to see if this tileset source's ID is "buildings" though :(
                    allBuildingTileInfos.Add(tileInfo);
            }

            return allBuildingTileInfos;
        }
    }
    
    public TileDatabase()
    {
        InjectionManager.Register(this);
    }

    ~TileDatabase()
    {
        InjectionManager.Deregister(this);
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
                var tileCoords = source.GetTileId(j);
                var tileData = source.GetTileData(tileCoords, 0).GetCustomData("data").Obj as CustomTileData;

                if (tileData == null)
                    continue;
                
                TileInfo tileInfo = new TileInfo();
                tileInfo.tileSet = tileSet;
                tileInfo.sourceId = sourceId;
                
                tileInfo.tileCoords = source.GetTileId(j);
                tileInfo.tileTexture = new AtlasTexture();
                tileInfo.tileTexture.Atlas = source.Texture;
                tileInfo.tileTexture.FilterClip = true;
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
            if (tileInfo.tileData?.canBePlacedOn?.Contains(customTileData) != true)
                continue;
            
            compatibleTileInfos.Add(tileInfo);
        }
        return compatibleTileInfos;
    }

    public AtlasTexture GetTileTexture(CustomTileData customTileData)
    {
        foreach (var tileInfo in tileInfos)
        {
            if (tileInfo.tileData == customTileData)
                return tileInfo.tileTexture;
        }

        return null;
    }

    public TileInfo GetTileInfoForCustomTileData(CustomTileData customTileData)
    {
        foreach (var tileInfo in AllTileInfos)
        {
            if (tileInfo.tileData == customTileData)
                return tileInfo;
        }
        return null;
    }

    public TileInfo GetTileInfoForAtlasCoords(int sourceId, Vector2I atlasCoords)
    {
        foreach (var tileInfo in tileInfos)
        {
            if (tileInfo.sourceId == sourceId && tileInfo.tileCoords == atlasCoords)
                return tileInfo;
        }
        return null;
    }
}