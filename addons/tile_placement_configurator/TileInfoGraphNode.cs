using Godot;

[Tool]
public partial class TileInfoGraphNode : GraphNode
{
    [Export]
    private TextureRect textureRect;

    public TileInfoGraph graph;

    public CustomTileData selectedTileData;

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

    public void OnDeleteButton()
    {
        selectedTileData.canBePlacedOn.Remove(tileData);
        ResourceSaver.Save(selectedTileData);

        graph.OnPlacedOnUpdated(this);
    }
}