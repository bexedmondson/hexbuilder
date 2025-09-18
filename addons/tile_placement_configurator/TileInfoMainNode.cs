using System.Collections.Generic;
using Godot;

[Tool]
public partial class TileInfoMainNode : TileInfoGraphNode
{
    [Export]
    private OptionButton optionButton;

    public void SetupOptionDropdown()
    {
        var dataDir = DirAccess.Open("res://data");
        this.optionButton.AddItem("");
        optionButton.SetItemDisabled(0, true);
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

    public void OnCanPlacedOnUpdated()
    {
        for (int i = 0; i < optionButton.ItemCount; i++)
        {
            optionButton.SetItemDisabled(i, customTileData.canBePlacedOn.Contains(optionButton.GetItemMetadata(i).Obj as CustomTileData));
        }
    }

    public void OnAddCanPlaceOnButton()
    {
        if (optionButton.IsItemDisabled(optionButton.GetSelected()))
            return;
        
        var selectedTileData = optionButton.GetSelectedMetadata().Obj as CustomTileData;
        customTileData.canBePlacedOn.Add(selectedTileData);
        ResourceSaver.Save(customTileData);

        graph.OnPlacedOnUpdated(this);

        OnCanPlacedOnUpdated();
    }
}