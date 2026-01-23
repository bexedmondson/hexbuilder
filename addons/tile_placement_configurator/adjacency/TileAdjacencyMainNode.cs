using System.Collections.Generic;
using Godot;

[Tool]
public partial class TileAdjacencyMainNode : GraphNode
{
    [Export]
    private TextureRect textureRect;
    
    [Export]
    private OptionButton optionButton;

    public TileAdjacencyGraph graph;

    private CustomTileData tileData;
    public CustomTileData customTileData
    {
        get { return tileData; }
        set
        {
            tileData = value;
            OnCustomTileDataSet();
        }
    }
    
    private Dictionary<string, EditorProperty> propertyEditors = new();
    
    public void OnCustomTileDataSet()
    {
        Title = tileData.GetFileName();
        textureRect.Texture ??= EditorTileDatabase.GetTileTexture(customTileData);
    }

    public void SetupOptionDropdown()
    {
        var dataDir = DirAccess.Open("res://data");
        this.optionButton.AddItem("");
        optionButton.SetItemDisabled(0, true);
        DirContents(dataDir);
    }
    
    public void DirContents(DirAccess dir)
    {
        List<CustomTileData> adjacentTileData = new();
        foreach (var adjacency in customTileData.adjacencies)
        {
            adjacentTileData.Add(adjacency.requiredTile);
        }
        
        if (dir != null)
        {
            dir.ListDirBegin();

            Dictionary<string, CustomTileData> optionsToSort = new();
            
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    AddSortedOptions(optionsToSort, optionButton, adjacentTileData);
                    
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
                AddSortedOptions(optionsToSort, optionButton, adjacentTileData);
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }
    }

    private void AddSortedOptions(Dictionary<string, CustomTileData> optionsToSort, OptionButton button, List<CustomTileData> adjacentTileData)
    {
        List<string> sortedOptions = new List<string>(optionsToSort.Keys);
        sortedOptions.Sort();
                    
        foreach (var option in sortedOptions)
        {
            button.AddItem(option);
            button.SetItemMetadata(button.ItemCount - 1, optionsToSort[option]);
            if (adjacentTileData.Contains(optionsToSort[option]))
                button.SetItemDisabled(button.ItemCount - 1, true);
        }
    }

    public void OnAdjacenciesUpdated()
    {
        List<CustomTileData> adjacentTileData = new();
        foreach (var adjacency in customTileData.adjacencies)
        {
            adjacentTileData.Add(adjacency.requiredTile);
        }
        
        for (int i = 0; i < optionButton.ItemCount; i++)
        {
            optionButton.SetItemDisabled(i, adjacentTileData.Contains(optionButton.GetItemMetadata(i).Obj as CustomTileData));
        }
    }

    public void OnAddAdjacencyButton()
    {
        if (optionButton.IsItemDisabled(optionButton.GetSelected()))
            return;
        
        var selectedTileData = optionButton.GetSelectedMetadata().Obj as CustomTileData;
        var newAdjacency = new AdjacencyConfig();
        newAdjacency.requiredTile = selectedTileData;
        newAdjacency.distance = 1;
        customTileData.adjacencies.Add(newAdjacency);
        ResourceSaver.Save(customTileData);

        graph.OnNodeDataUpdated();

        OnAdjacenciesUpdated();
    }
}