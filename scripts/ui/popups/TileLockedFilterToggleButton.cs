using Godot;

public partial class TileLockedFilterToggleButton : AbstractTileFilterToggleButton
{
    public override bool DoesFilterTileData(CustomTileData tileData)
    {
        if (!this.IsPressed())
            return false;

        return !tileData.IsUnlocked();
    }
}
