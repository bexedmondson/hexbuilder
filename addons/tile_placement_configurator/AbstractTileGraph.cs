
using Godot;

[Tool]
public abstract partial class AbstractTileGraph : GraphEdit
{
    public abstract void OnTreeItemSelected(CustomTileData selectedTileData);
    
    public abstract void OnAdjacencyUpdated();
}
