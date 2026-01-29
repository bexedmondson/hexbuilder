using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Extensions
{
    public static CustomTileData GetCellCustomData(this TileMapLayer tileMapLayer, Vector2I cell)
    {
        var tileData = tileMapLayer.GetCellTileData(cell);
        if (tileData == null)
            return null;
        
        //getting custom data from data layer
        var customTileData = tileData.GetCustomData("data");

        if (customTileData.Obj is CustomTileData hexCustomTileData)
            return hexCustomTileData;
        
        return null;
    }

    public static bool IsCellEmpty(this TileMapLayer tileMapLayer, Vector2I cell)
    {
        //as per https://docs.godotengine.org/en/4.4/classes/class_tilemaplayer.html:
        //"A cell is considered empty if its source identifier equals -1, its atlas coordinate identifier is Vector2(-1, -1)
        //and its alternative identifier is -1."

        return tileMapLayer.GetCellSourceId(cell) == -1
               && tileMapLayer.GetCellAtlasCoords(cell) == (Vector2I.One * -1)
               && tileMapLayer.GetCellAlternativeTile(cell) == -1;
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }
}