
using Godot;

[Tool]
public abstract partial class AbstractTileGraph : GraphEdit
{
    [Export]
    private Tree tree;
    
    public abstract void OnTreeItemSelected(CustomTileData selectedTileData);
    
    public abstract void OnNodeDataUpdated();

    public async void SelectItem(CustomTileData customTileData)
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        
        var child = tree.GetRoot();

        while (child != null)
        {
            if (child.GetMetadata(0).Obj is CustomTileData tileData && tileData == customTileData)
            {
                tree.SetSelected(child, 0);
                tree.ScrollToItem(child, true);
                break;
            }

            child = child.GetNextInTree();
        }
    }
}
