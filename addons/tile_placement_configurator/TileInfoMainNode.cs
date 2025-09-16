using Godot;
using hexbuilder.addons.tile_placement_configurator;

[Tool]
public partial class TileInfoMainNode : TileInfoGraphNode
{
    [Export]
    private Button addCanPlaceOnButton;

    [Export]
    private OptionButton optionButton;

    private TileInfoGraph graph;

    public void Setup(TileInfoGraph graphEdit)
    {
        graph = graphEdit;
        var dataDir = DirAccess.Open("res://data");
        this.optionButton.AddItem("");
        DirContents(dataDir);
    }
    
    public void DirContents(DirAccess dir)
    {
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    optionButton.AddSeparator(fileName);
                    DirContents(DirAccess.Open(dir.GetCurrentDir() +"/"+ fileName));
                }
                else
                {
                    optionButton.AddItem(fileName.TrimSuffix("." + fileName.GetExtension()));
                    var tileDataResource = ResourceLoader.Load(dir.GetCurrentDir() + "/" + fileName) as CustomTileData;
                    optionButton.SetItemMetadata(optionButton.ItemCount - 1, tileDataResource);
                    if (customTileData.canBePlacedOn.Contains(tileDataResource))
                        optionButton.SetItemDisabled(optionButton.ItemCount - 1, true);
                }

                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }
    }

    public void OnAddCanPlaceOnButton()
    {
        var selectedTileData = optionButton.GetSelectedMetadata().Obj as CustomTileData;
        customTileData.canBePlacedOn.Add(selectedTileData);
        ResourceSaver.Save(customTileData);

        graph.OnPlacedOnUpdated();

        optionButton.SetItemDisabled(optionButton.GetSelected(), true);
    }
}