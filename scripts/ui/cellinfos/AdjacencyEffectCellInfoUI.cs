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
    private WorkplaceManager workplaceManager;

    public void Update(CustomTileData tileData, Vector2I cell)
    {
        mapController ??= InjectionManager.Get<MapController>();
        workplaceManager = InjectionManager.Get<WorkplaceManager>();
        
        if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
            return;

        if (tileData.IsWorkplace && workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplaceState) && workplaceState.workerCount == 0)
            return;
        
        var adjacentCells = mapController.BaseMapLayer.GetSurroundingCells(cell);

        foreach (var adjacentCell in adjacentCells)
        {
            if (mapController.GetCellStatus(adjacentCell) != CellStatus.Unlocked)
                continue;
            
            var adjacentData = mapController.BaseMapLayer.GetCellCustomData(adjacentCell);
            CurrencySum receivedEffect;
            
            if (adjacentData.IsWorkplace)
            {
                if (!workplaceManager.TryGetWorkplaceAtLocation(adjacentCell, out var neighbourWorkplace))
                {
                    GD.PushError("workplace data from tile data but no workplace found here?? something's gone horribly wrong");
                    continue;
                }

                receivedEffect = neighbourWorkplace.GetWorkerDependentAdjacencyEffects(tileData);

                int total = 0;
                foreach (var kvp in receivedEffect)
                {
                    total += kvp.Value;
                }
                
                if (total == 0) //if workers (or lack thereof) actually causing zero effect, carry on
                    continue;
            } 
            else if (!tileData.TryGetAdjacencyEffectFromTileData(adjacentData, out receivedEffect))
            {
                continue; //if not a workplace and therefore not undergoing above checks, and if no adjacence effect, carry on
            }

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
