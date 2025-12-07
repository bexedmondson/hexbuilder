
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class TileInfoGraph : AbstractTileGraph
{
    [Export]
    private PackedScene tileGraphMainNodeScene;

    [Export]
    private PackedScene tileGraphNodeScene;

    private TileInfoMainNode tileGraphMainNode;

    private Dictionary<CustomTileData, TileInfoGraphNode> nodes = new();

    public override void OnTreeItemSelected(CustomTileData selectedTileData)
    {
        this.ClearConnections();
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            if (this.GetChild(i) is TileInfoGraphNode graphNode)
            {
                nodes.Remove(graphNode.customTileData);
                this.GetChild(i).Free();
            }
        }

        tileGraphMainNode = tileGraphMainNodeScene.Instantiate() as TileInfoMainNode;
        tileGraphMainNode.graph = this;
        tileGraphMainNode.customTileData = selectedTileData;
        tileGraphMainNode.SetupOptionDropdown();
        tileGraphMainNode.ResetSize();
        this.AddChild(tileGraphMainNode);
        nodes[selectedTileData] = tileGraphMainNode;
        
        tileGraphMainNode.SetSlotEnabledRight(0, true);

        TileInfoGraphNode firstNode = null;
        TileInfoGraphNode lastNode = null;

        for (int i = 0; i < selectedTileData.canBePlacedOn.Count; i++)
        {
            var canBePlacedOn = selectedTileData.canBePlacedOn[i];
            var placedOnNode = AddTileNode(canBePlacedOn, tileGraphMainNode);

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

        ResetView(tileGraphMainNode);
    }

    private TileInfoGraphNode AddTileNode(CustomTileData customTileData, TileInfoGraphNode sourceNode, bool shouldBeEditable = true)
    {
        TileInfoGraphNode newGraphNode = tileGraphNodeScene.Instantiate() as TileInfoGraphNode;
        newGraphNode.graph = this;
        newGraphNode.selectedTileData = sourceNode.customTileData;
        newGraphNode.customTileData = customTileData;
        newGraphNode.SetupAsEditable(shouldBeEditable);
        newGraphNode.ResetSize();
        this.AddChild(newGraphNode);
        nodes[customTileData] = newGraphNode;

        sourceNode.SetSlotEnabledRight(0, true);
        newGraphNode.SetSlotEnabledLeft(0, true);

        if (customTileData.canBePlacedOn == null || customTileData.canBePlacedOn.Count == 0)
        {
            this.ConnectNode(sourceNode.Name, 0, newGraphNode.Name, 0);
            return newGraphNode;
        }

        foreach (var canBePlacedOn in customTileData.canBePlacedOn)
        {
            if (nodes.TryGetValue(canBePlacedOn, out var preexistingNode))
            {
                newGraphNode.SetSlotEnabledRight(0, true);
                preexistingNode.SetSlotEnabledLeft(0, true);
                this.ConnectNode(newGraphNode.Name, 0, preexistingNode.Name, 0);
            }
            else
            {
                AddTileNode(canBePlacedOn, newGraphNode, false);
            }
        }

        this.ConnectNode(sourceNode.Name, 0, newGraphNode.Name, 0);
        return newGraphNode;
    }

    private void ResetView(TileInfoMainNode mainNode)
    {
        Zoom = 0.8f;
        ScrollOffset = Vector2.One * -10;
    }

    public override void OnNodeDataUpdated()
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
                AddTileNode(canBePlacedOn, tileGraphMainNode);
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
                nodes.Remove(tileInfoGraphNode.customTileData);
                tileInfoGraphNode.QueueFree();
            }
        }
        
        ArrangeNodes();
        
        tileGraphMainNode.OnCanPlacedOnUpdated();
    }
}