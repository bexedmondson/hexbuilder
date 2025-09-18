
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class TileInfoGraph : GraphEdit
{
    [Export]
    private PackedScene tileGraphMainNodeScene;

    [Export]
    private PackedScene tileGraphNodeScene;

    private TileInfoMainNode tileGraphMainNode;

    public void OnTreeItemSelected(CustomTileData selectedTileData)
    {
        this.ClearConnections();
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            if (this.GetChild(i) is TileInfoGraphNode)
                this.GetChild(i).Free();
        }

        tileGraphMainNode = tileGraphMainNodeScene.Instantiate() as TileInfoMainNode;
        tileGraphMainNode.graph = this;
        tileGraphMainNode.customTileData = selectedTileData;
        tileGraphMainNode.SetupOptionDropdown();
        tileGraphMainNode.ResetSize();
        this.AddChild(tileGraphMainNode);
        
        tileGraphMainNode.SetSlotEnabledRight(0, true);

        TileInfoGraphNode firstNode = null;
        TileInfoGraphNode lastNode = null;

        for (int i = 0; i < selectedTileData.canBePlacedOn.Count; i++)
        {
            var canBePlacedOn = selectedTileData.canBePlacedOn[i];
            var placedOnNode = AddTileNode(canBePlacedOn);

            if (i == 0)
                firstNode = placedOnNode;
            else if (i == selectedTileData.canBePlacedOn.Count - 1)
                lastNode = placedOnNode;
        }
        
        ArrangeNodes();

        if (firstNode != null && lastNode != null)
        {
            tileGraphMainNode.PositionOffset = new Vector2(tileGraphMainNode.PositionOffset.X, (lastNode.PositionOffset.Y - firstNode.PositionOffset.Y) / 2 + firstNode.PositionOffset.Y);
        }
    }

    private TileInfoGraphNode AddTileNode(CustomTileData customTileData)
    {
        TileInfoGraphNode graphNode = tileGraphNodeScene.Instantiate() as TileInfoGraphNode;
        graphNode.graph = this;
        graphNode.selectedTileData = tileGraphMainNode.customTileData;
        graphNode.customTileData = customTileData;
        graphNode.ResetSize();
        this.AddChild(graphNode);

        graphNode.SetSlotEnabledLeft(0, true);

        this.ConnectNode(tileGraphMainNode.Name, 0, graphNode.Name, 0);
        return graphNode;
    }

    public void OnPlacedOnUpdated(TileInfoGraphNode updateSourceNode)
    {
        var connectionListFromNode = GetConnectionListFromNode(tileGraphMainNode.Name);
        foreach (var canBePlacedOn in tileGraphMainNode.customTileData.canBePlacedOn)
        {
            Dictionary foundConnection = null;
            foreach (var connection in connectionListFromNode)
            {
                var obj = connection["to_node"].Obj;
                var foundChild = this.GetChildren().FirstOrDefault(child => child.Name.Equals(connection["to_node"].Obj));
                if (foundChild == null || foundChild is not TileInfoGraphNode tileInfoGraphNode)
                {
                    //TODO remove?? how did we get here
                }
                else if (tileInfoGraphNode.customTileData == canBePlacedOn)
                {
                    foundConnection = connection;
                    break;
                }
            }

            if (foundConnection != null)
                connectionListFromNode.Remove(foundConnection);
            else
                AddTileNode(canBePlacedOn);
        }

        foreach (var leftoverConnection in connectionListFromNode)
        {
            var foundChild = this.GetChildren().FirstOrDefault(child => child.Name.Equals(leftoverConnection["to_node"].Obj));
            if (foundChild == null || foundChild is not TileInfoGraphNode tileInfoGraphNode)
            {
                DisconnectNode((StringName)leftoverConnection["from_node"], (int)leftoverConnection["from_port"], (StringName)leftoverConnection["to_node"], (int)leftoverConnection["to_port"]);
            }
            else
            {
                this.RemoveChild(tileInfoGraphNode);
                tileInfoGraphNode.QueueFree();
            }
        }
        
        ArrangeNodes();
        
        tileGraphMainNode.OnCanPlacedOnUpdated();
    }
}