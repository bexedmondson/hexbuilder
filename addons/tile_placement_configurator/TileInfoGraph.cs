using System.Collections.Generic;
using Godot;

namespace hexbuilder.addons.tile_placement_configurator;

[Tool]
public partial class TileInfoGraph : GraphEdit
{
    public void OnTreeItemSelected(CustomTileData selectedTileData)
    {
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            if (this.GetChild(i) is GraphNode)
                this.GetChild(i).QueueFree();
        }
        
        GraphNode mainNode = AddTileNode(selectedTileData);

        Label mainLabel = new Label();
        mainLabel.Text = "can be placed on";
        mainNode.AddChild(mainLabel);
        
        mainNode.SetSlotEnabledRight(0, true);
        
        foreach (var canBePlacedOn in selectedTileData.canBePlacedOn)
        {
            var placedOnNode = AddTileNode(canBePlacedOn);

            Label acceptsLabel = new Label();
            acceptsLabel.Text = "accepts";
            placedOnNode.AddChild(acceptsLabel);
            
            placedOnNode.SetSlotEnabledLeft(0, true);

            this.ConnectNode(mainNode.Name, 0, placedOnNode.Name, 0);
        }
        
        ArrangeNodes();
    }

    private GraphNode AddTileNode(CustomTileData customTileData)
    {
        GraphNode mainNode = new GraphNode();
        string fileName = customTileData.ResourcePath.GetFile();
        fileName = fileName.TrimSuffix("." + customTileData.ResourcePath.GetFile().GetExtension());
        mainNode.Title = fileName;
        mainNode.ResetSize();
        this.AddChild(mainNode);
        return mainNode;
    }
}