using Godot;

public partial class TileTextureRect : TextureRect
{
    [Export]
    private ShaderMaterial greyscaleMaterial;

    public void SetTile(TileDatabase.TileInfo tileInfo)
    {
        Texture = tileInfo.tileTexture;
        Material = tileInfo.tileData.IsUnlocked() ? null : greyscaleMaterial;
    }
    
    public void SetTile(CustomTileData tileData)
    {
        Texture = InjectionManager.Get<TileDatabase>().GetTileTexture(tileData);
        Material = tileData.IsUnlocked() ? null : greyscaleMaterial;
    }

    public void Cleanup()
    {
        Texture = null;
        Material = null;
    }
}
