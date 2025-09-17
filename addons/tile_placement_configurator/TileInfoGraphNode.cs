using Godot;

public partial class TileInfoGraphNode : GraphNode
{
    public CustomTileData customTileData;

    public void AddTexture()
    {
        TextureRect textureRect = new TextureRect();
        textureRect.Texture = InjectionManager.Get<TileDatabase>().GetTileTexture(customTileData);
        textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
        textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        textureRect.CustomMinimumSize = Vector2.One * 100;
        this.AddChild(textureRect);
    }
}