using Godot;

[Tool]
public partial class TileInfoGraphNode : GraphNode
{
    [Export]
    private TextureRect textureRect;

    private CustomTileData tileData;
    public CustomTileData customTileData
    {
        get => tileData;
        set
        {
            tileData = value;
            OnCustomTileDataSet();
        }
    }

    public void OnCustomTileDataSet()
    {
        Title = customTileData.GetFileName();
        textureRect.Texture = InjectionManager.Get<TileDatabase>().GetTileTexture(customTileData);
    }
}