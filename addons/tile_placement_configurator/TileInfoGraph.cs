
using System.Linq;
using Godot;

[Tool]
public partial class TileInfoGraph : GraphEdit
{
    [Export]
    private PackedScene tileGraphNodeScene;

    private TileInfoMainNode tileGraphMainNode;

    public void OnTreeItemSelected(CustomTileData selectedTileData)
    {
        this.ClearConnections();
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            if (this.GetChild(i) is TileInfoGraphNode)
                this.GetChild(i).QueueFree();
        }

        tileGraphMainNode = tileGraphNodeScene.Instantiate() as TileInfoMainNode;
        string fileName = selectedTileData.ResourcePath.GetFile();
        fileName = fileName.TrimSuffix("." + selectedTileData.ResourcePath.GetFile().GetExtension());
        tileGraphMainNode.Title = fileName;
        tileGraphMainNode.customTileData = selectedTileData;
        tileGraphMainNode.Setup(this);
        tileGraphMainNode.AddTexture();
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
        TileInfoGraphNode graphNode = new TileInfoGraphNode();
        string fileName = customTileData.ResourcePath.GetFile();
        fileName = fileName.TrimSuffix("." + customTileData.ResourcePath.GetFile().GetExtension());
        graphNode.Title = fileName;
        graphNode.customTileData = customTileData;
        graphNode.AddTexture();
        graphNode.ResetSize();
        this.AddChild(graphNode);

        graphNode.SetSlotEnabledLeft(0, true);

        this.ConnectNode(tileGraphMainNode.Name, 0, graphNode.Name, 0);
        return graphNode;
    }

    public void OnPlacedOnUpdated()
    {
        var connectionListFromNode = GetConnectionListFromNode(tileGraphMainNode.Name);
        foreach (var canBePlacedOn in tileGraphMainNode.customTileData.canBePlacedOn)
        {
            bool hasConnection = false;
            foreach (var connection in connectionListFromNode)
            {
                var foundChild = this.GetChildren().FirstOrDefault(child => child.Equals(connection["to_node"].Obj));
                if (foundChild == null)
                {
                    //TODO remove connection
                }
                else
                {
                    hasConnection = true;
                    break;
                }
            }

            if (!hasConnection)
                AddTileNode(canBePlacedOn);
        }
        
        ArrangeNodes();
    }
}