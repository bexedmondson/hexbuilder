using Godot;

namespace hexbuilder.addons.tile_placement_configurator;

[Tool]
public partial class TileSelectionTree : Tree
{
    [Export]
    private TileInfoGraph tileInfoGraph;

    [Export]
    private TileSet tileSet;
    
    public override void _Ready()
    {
        base._Ready();
        
        InjectionManager.Get<TileDatabase>().AddTileSetTileData(tileSet);

        var dataDir = DirAccess.Open("res://data");
        var root = this.CreateItem();
        DirContents(dataDir, root);
    }
    
    public void DirContents(DirAccess dir, TreeItem parent)
    {
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                var treeItem = CreateItem(parent);
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

    public void OnTreeItemSelected()
    {
        var selected = this.GetSelected();
        var metadata = selected.GetMetadata(0).Obj;
        if (metadata is not CustomTileData tileData)
            return;
        
        tileInfoGraph.OnTreeItemSelected(tileData);
    }
}