using System.Collections.Generic;
using Godot;

public partial class WorkplaceCellInfoUIContainer : Control
{
    [Export]
    private TileMapLayer baseTileMapLayer;

    [Export]
    private PackedScene cellInfoUIScene;

    private Dictionary<Vector2I, WorkplaceCellInfoUI> workplaceCellInfoUis = new();

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Get<EventDispatcher>().Add<WorkplaceUpdatedEvent>(OnWorkplaceUpdated);
    }

    private void OnWorkplaceUpdated(WorkplaceUpdatedEvent workplaceUpdatedEvent)
    {
        foreach (var removedWorkplace in workplaceUpdatedEvent.removedWorkplaces)
        {
            var removedWorkplaceLocation = removedWorkplace.location;
            if (workplaceCellInfoUis.TryGetValue(removedWorkplaceLocation, out var removedWorkplaceInfoUI))
            {
                this.RemoveChild(removedWorkplaceInfoUI);
                removedWorkplaceInfoUI.QueueFree();
                workplaceCellInfoUis.Remove(removedWorkplaceLocation);
            }
        }

        foreach (var workplace in workplaceUpdatedEvent.newOrChangedWorkplaces)
        {
            if (workplaceCellInfoUis.TryGetValue(workplace.location, out var changedWorkplaceUI))
            {
                changedWorkplaceUI.UpdateWorkerCountLabel(workplace.workerCount, workplace.capacity);
                changedWorkplaceUI.SetWorkerMaxBonusEffect(workplace.workerCount == workplace.capacity
                                                           && workplace.tileData.TryGetComponent(out MaximumWorkerProductionBonusComponent _));
            }
            else
            {
                var newWorkplaceUI = cellInfoUIScene.Instantiate<WorkplaceCellInfoUI>();
                newWorkplaceUI.UpdateWorkerCountLabel(workplace.workerCount, workplace.capacity);
                newWorkplaceUI.SetWorkerMaxBonusEffect(workplace.workerCount == workplace.capacity
                                                           && workplace.tileData.TryGetComponent(out MaximumWorkerProductionBonusComponent _));
                newWorkplaceUI.GlobalPosition = baseTileMapLayer.ToGlobal(baseTileMapLayer.MapToLocal(workplace.location));
                workplaceCellInfoUis[workplace.location] = newWorkplaceUI;
                this.AddChild(newWorkplaceUI);
            }
        }
    }
}

