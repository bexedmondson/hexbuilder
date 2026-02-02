#if TOOLS
using System;
using Godot;

[Tool]
public partial class TileSelectionTree : Tree
{
    [Export]
    private AbstractTileGraph tileGraph;

    [Export]
    private TilePluginParentNode tilePluginParentNode;

    public override void _EnterTree()
    {
        base._EnterTree();
        
        var dataDir = DirAccess.Open("res://data");
        var root = this.CreateItem();
        DirContents(dataDir, root);
        SortTree(root);
    }
    
    public void DirContents(DirAccess dir, TreeItem parent)
    {
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                int index = GetNewItemIndex(parent, fileName);
                var treeItem = CreateItem(parent, index);
                treeItem.SetText(0, fileName.TrimSuffix("." + fileName.GetExtension()));

                if (dir.CurrentIsDir())
                    DirContents(DirAccess.Open(dir.GetCurrentDir() +"/"+ fileName), treeItem);
                else
                    treeItem.SetMetadata(0, ResourceLoader.Load(dir.GetCurrentDir() +"/"+ fileName));
                
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }
    }

    private int GetNewItemIndex(TreeItem parent, string newItemName)
    {
        var items = parent.GetChildren();
        int i = 0;
        for (; i < items.Count; i++)
        {
            var sibling = items[i];
            if (string.Compare(sibling.GetText(0), (newItemName), StringComparison.InvariantCultureIgnoreCase) > 0)
                break;
        }

        return i;
    }

    private void SortTree(TreeItem parent)
    {
        var children = parent.GetChildren();
        children.Sort();
        
        foreach (var childItem in children)
        {
            SortTree(childItem);
        }
    }

    public void OnTreeItemSelected()
    {
        var selected = this.GetSelected();
        var metadata = selected.GetMetadata(0).Obj;
        if (metadata is not CustomTileData tileData)
        {
            tilePluginParentNode.ShowGenericResourceInspector(metadata);
            return;
        }
        
        tilePluginParentNode.HideGenericResourceInspector();
        tileGraph.OnTreeItemSelected(tileData);
    }
}
#endif