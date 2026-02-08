using Godot;

public abstract partial class AbstractTileFilterToggleButton : CheckButton
{
    public abstract bool DoesFilterTileData(CustomTileData tileData);
}
