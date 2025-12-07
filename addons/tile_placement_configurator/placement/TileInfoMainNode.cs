using System.Collections.Generic;
using Godot;
using Godot.Collections;

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
            
            System.Collections.Generic.Dictionary<string, CustomTileData> optionsToSort = new();
            
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    AddSortedOptions(optionsToSort, optionButton, customTileData.canBePlacedOn);
                    
                    optionButton.AddSeparator(fileName);
                    DirContents(DirAccess.Open(dir.GetCurrentDir() +"/"+ fileName));
                    optionsToSort.Clear();
                }
                else
                {
                    var tileDataResource = ResourceLoader.Load(dir.GetCurrentDir() + "/" + fileName) as CustomTileData;
                    optionsToSort[fileName.TrimSuffix("." + fileName.GetExtension())] = tileDataResource;
                }

                fileName = dir.GetNext();
            }
            
            if (optionsToSort.Count > 0)
            {
                AddSortedOptions(optionsToSort, optionButton, customTileData.canBePlacedOn);
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }
    }
    
    private void AddSortedOptions(System.Collections.Generic.Dictionary<string, CustomTileData> optionsToSort, OptionButton button, Array<CustomTileData> canBePlacedOnTileData)
    {
        List<string> sortedOptions = new List<string>(optionsToSort.Keys);
        sortedOptions.Sort();
                    
        foreach (var option in sortedOptions)
        {
            button.AddItem(option);
            button.SetItemMetadata(button.ItemCount - 1, optionsToSort[option]);
            if (canBePlacedOnTileData.Contains(optionsToSort[option]))
                button.SetItemDisabled(button.ItemCount - 1, true);
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

        graph.OnNodeDataUpdated();

        OnCanPlacedOnUpdated();
    }
}