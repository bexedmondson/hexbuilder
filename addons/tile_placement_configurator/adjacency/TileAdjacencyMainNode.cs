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
        textureRect.Texture = InjectionManager.Get<TileDatabase>().GetTileTexture(tileData);
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
                    if (adjacentTileData.Contains(tileDataResource))
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

        graph.OnAdjacencyUpdated();

        OnAdjacenciesUpdated();
    }
}