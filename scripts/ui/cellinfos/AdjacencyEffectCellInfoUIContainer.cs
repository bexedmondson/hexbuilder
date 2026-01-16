using System.Collections.Generic;
using Godot;

public partial class AdjacencyEffectCellInfoUIContainer : Node2D
{
    [Export]
    private PackedScene adjacencyEffectCellInfoUIScene;
    
    private MapController mapController;

    private Dictionary<Vector2I, AdjacencyEffectCellInfoUI> infoUIs = new();

    private bool isOn = false;

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Get<EventDispatcher>().Add<NextTurnEvent>(OnNextTurn);
    }

    private void OnNextTurn(NextTurnEvent _)
    {
        mapController ??= InjectionManager.Get<MapController>();

        var mapLayer = mapController.BaseMapLayer;
        
        foreach (var usedCell in mapLayer.GetUsedCells())
        {
            if (mapController.GetCellStatus(usedCell) != CellStatus.Unlocked)
                continue;
            
            if (infoUIs.TryGetValue(usedCell, out var infoUI))
            {
                infoUI.Update(mapLayer.GetCellCustomData(usedCell), usedCell);
            }
            else
            {
                var newInfoUI = adjacencyEffectCellInfoUIScene.Instantiate<AdjacencyEffectCellInfoUI>();
                newInfoUI.Update(mapLayer.GetCellCustomData(usedCell), usedCell);
                newInfoUI.GlobalPosition = mapLayer.ToGlobal(mapLayer.MapToLocal(usedCell));
                newInfoUI.Name = $"{usedCell} AdjacencyEffectCellInfoUIContainer";
                infoUIs[usedCell] = newInfoUI;
                this.AddChild(newInfoUI);
            }
        }
    }
}

