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
}