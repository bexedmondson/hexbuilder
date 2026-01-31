using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class TileAdjacencyGraph : AbstractTileGraph
{
    [Export]
    private PackedScene tileAdjacencyMainNodeScene;

    [Export]
    private PackedScene tileAdjacencyNodeScene;

    private TileAdjacencyMainNode tileAdjacencyMainNode;

    public override void OnTreeItemSelected(CustomTileData selectedTileData)
    {
        this.ClearConnections();
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            if (this.GetChild(i) is TileAdjacencyGraphNode || this.GetChild(i) is TileAdjacencyMainNode)
                this.GetChild(i).Free();
        }

        tileAdjacencyMainNode = tileAdjacencyMainNodeScene.Instantiate() as TileAdjacencyMainNode;
        tileAdjacencyMainNode.SetCustomTileData(selectedTileData);
        tileAdjacencyMainNode.graph = this;
        tileAdjacencyMainNode.SetupOptionDropdown();
        tileAdjacencyMainNode.ResetSize();
        this.AddChild(tileAdjacencyMainNode);
        
        tileAdjacencyMainNode.SetSlotEnabledRight(0, true);

        TileAdjacencyGraphNode firstNode = null;
        TileAdjacencyGraphNode lastNode = null;

        for (int i = 0; i < selectedTileData.adjacencies.Count; i++)
        {
            var adjacency = selectedTileData.adjacencies[i];
            var adjacencyNode = AddTileNode(adjacency);

            if (i == 0)
                firstNode = adjacencyNode;
            else if (i == selectedTileData.adjacencies.Count - 1)
                lastNode = adjacencyNode;
        }
        
        ArrangeNodes();

        if (firstNode != null && lastNode != null)
        {
            tileAdjacencyMainNode.PositionOffset = new Vector2(tileAdjacencyMainNode.PositionOffset.X, (lastNode.PositionOffset.Y - firstNode.PositionOffset.Y) / 2 + firstNode.PositionOffset.Y);
        }
    }

    private TileAdjacencyGraphNode AddTileNode(AdjacencyConfig adjacencyConfig)
    {
        TileAdjacencyGraphNode graphNode = tileAdjacencyNodeScene.Instantiate() as TileAdjacencyGraphNode;
        graphNode.selectedTileData = tileAdjacencyMainNode.tileData;
        graphNode.SetAdjacencyConfig(adjacencyConfig);
        graphNode.graph = this;
        graphNode.ResetSize();
        this.AddChild(graphNode);

        graphNode.SetSlotEnabledLeft(0, true);

        this.ConnectNode(tileAdjacencyMainNode.Name, 0, graphNode.Name, 0);
        return graphNode;
    }

    public override void OnNodeDataUpdated()
    {
        var connectionListFromNode = GetConnectionListFromNode(tileAdjacencyMainNode.Name);
        foreach (var adjacency in tileAdjacencyMainNode.tileData.adjacencies)
        {
            Dictionary foundConnection = null;
            foreach (var connection in connectionListFromNode)
            {
                var foundChild = this.GetChildren().FirstOrDefault(child => child.Name.Equals(connection["to_node"].Obj));
                if (foundChild == null || foundChild is not TileAdjacencyGraphNode tileAdjacencyGraphNode)
                {
                    //TODO remove?? how did we get here
                }
                else if (tileAdjacencyGraphNode.adjacencyConfig == adjacency)
                {
                    foundConnection = connection;
                    break;
                }
            }

            if (foundConnection != null)
                connectionListFromNode.Remove(foundConnection);
            else
                AddTileNode(adjacency);
        }

        foreach (var leftoverConnection in connectionListFromNode)
        {
            var foundChild = this.GetChildren().FirstOrDefault(child => child.Name.Equals(leftoverConnection["to_node"].Obj));
            if (foundChild == null || foundChild is not TileAdjacencyGraphNode tileAdjacencyGraphNode)
            {
                DisconnectNode((StringName)leftoverConnection["from_node"], (int)leftoverConnection["from_port"], (StringName)leftoverConnection["to_node"], (int)leftoverConnection["to_port"]);
            }
            else
            {
                this.RemoveChild(tileAdjacencyGraphNode);
                tileAdjacencyGraphNode.QueueFree();
            }
        }
        
        ArrangeNodes();
        
        tileAdjacencyMainNode.OnAdjacenciesUpdated();
    }
}