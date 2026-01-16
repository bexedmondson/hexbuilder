using Godot;

public partial class AdjacencyEffectCellInfoUI : Node2D
{
    [Export]
    private AdjacencyEffectAnimationTree topLeft;

    [Export]
    private AdjacencyEffectAnimationTree top;

    [Export]
    private AdjacencyEffectAnimationTree topRight;

    [Export]
    private AdjacencyEffectAnimationTree bottomLeft;

    [Export]
    private AdjacencyEffectAnimationTree bottom;

    [Export]
    private AdjacencyEffectAnimationTree bottomRight;
    
    private MapController mapController;

    public void Update(CustomTileData tileData, Vector2I cell)
    {
        mapController ??= InjectionManager.Get<MapController>();
        
        if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
            return;
        
        var adjacentCells = mapController.BaseMapLayer.GetSurroundingCells(cell);

        foreach (var adjacentCell in adjacentCells)
        {
            if (mapController.GetCellStatus(adjacentCell) != CellStatus.Unlocked)
                continue;
            
            var adjacentData = mapController.BaseMapLayer.GetCellCustomData(adjacentCell);
            if (!tileData.TryGetAdjacencyEffectFromTileData(adjacentData, out var receivedEffect))
                continue;

            var direction = cell - adjacentCell;

            switch (direction.X)
            {
                case -1:
                    if (direction.Y == 0)
                    {
                        foreach (var kvp in receivedEffect)
                        {
                            SetAnimationTree(topRight, kvp.Key, kvp.Value);
                        }
                        //GD.Print("topright");
                    }
                    else
                    {
                        foreach (var kvp in receivedEffect)
                        {
                            SetAnimationTree(top, kvp.Key, kvp.Value);
                        }
                        //GD.Print("top");
                    }
                    break;
                case 0:
                    if (direction.Y == 1)
                    {
                        foreach (var kvp in receivedEffect)
                        {
                            SetAnimationTree(topLeft, kvp.Key, kvp.Value);
                        }
                        //GD.Print("topleft");
                    }
                    else
                    {
                        foreach (var kvp in receivedEffect)
                        {
                            SetAnimationTree(bottomRight, kvp.Key, kvp.Value);
                        }
                        //GD.Print("bottomright");
                    }
                    break;
                case 1:
                    if (direction.Y == -1)
                    {
                        foreach (var kvp in receivedEffect)
                        {
                            SetAnimationTree(bottom, kvp.Key, kvp.Value);
                        }
                        //GD.Print("bottom");
                    }
                    else
                    {
                        foreach (var kvp in receivedEffect)
                        {
                            SetAnimationTree(bottomLeft, kvp.Key, kvp.Value);
                        }
                        //GD.Print("bottomleft");
                    }
                    break;
            }
        }
    }

    private void SetAnimationTree(AdjacencyEffectAnimationTree animationTree, CurrencyType currencyType, int amount)
    {
        animationTree.SetCurrencyType(currencyType);
        animationTree.SetIsDrawback(amount < 0);
        animationTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
    }
}
