using System.Collections.Generic;
using Godot;

public partial class TimedTaskCellInfoUIContainer : Control
{
    [Export]
    private TileMapLayer baseTileMapLayer;

    [Export]
    private PackedScene cellInfoUIScene;

    private Dictionary<Vector2I, TimedTaskCellInfoUI> timedTaskCellInfoUis = new();

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Get<EventDispatcher>().Add<TimedTaskStartedEvent>(OnTimedTaskStarted);
        InjectionManager.Get<EventDispatcher>().Add<TimedTaskUpdatedEvent>(OnTimedTaskUpdated);
        InjectionManager.Get<EventDispatcher>().Add<TimedTaskEndedEvent>(OnTimedTaskEnded);
    }

    private void OnTimedTaskStarted(TimedTaskStartedEvent timedTaskStartedEvent)
    {
        if (timedTaskCellInfoUis.ContainsKey(timedTaskStartedEvent.location))
        {
            GD.PushError("how did this happen");
            return;
        }

        var newTimedTaskCellInfoUI = cellInfoUIScene.Instantiate<TimedTaskCellInfoUI>();
        newTimedTaskCellInfoUI.UpdateWorkerCountLabel(timedTaskStartedEvent.workerCount, timedTaskStartedEvent.capacity);
        newTimedTaskCellInfoUI.UpdateTurnCountLabel(timedTaskStartedEvent.timedTask.turnDuration);
        newTimedTaskCellInfoUI.GlobalPosition = baseTileMapLayer.ToGlobal(baseTileMapLayer.MapToLocal(timedTaskStartedEvent.location));
        timedTaskCellInfoUis[timedTaskStartedEvent.location] = newTimedTaskCellInfoUI;
        this.AddChild(newTimedTaskCellInfoUI);
    }

    private void OnTimedTaskUpdated(TimedTaskUpdatedEvent timedTaskUpdatedEvent)
    {
        if (timedTaskCellInfoUis.TryGetValue(timedTaskUpdatedEvent.location, out var updatedJobUI))
        {
            updatedJobUI.UpdateTurnCountLabel(timedTaskUpdatedEvent.timedTask.turnDuration - timedTaskUpdatedEvent.timedTask.turnsElapsed);
            updatedJobUI.UpdateWorkerCountLabel(timedTaskUpdatedEvent.timedTask.workerCount, timedTaskUpdatedEvent.timedTask.workerCountRequirement);
            updatedJobUI.DoTurnAnim();
        }
    }
    
    private void OnTimedTaskEnded(TimedTaskEndedEvent timedTaskEndedEvent)
    {
        if (timedTaskCellInfoUis.TryGetValue(timedTaskEndedEvent.location, out var removedTimedTaskInfoUI))
        {
            removedTimedTaskInfoUI.AnimateOut(
                GetEndAnimationNameForJob(timedTaskEndedEvent),
                () =>
            {
                this.RemoveChild(removedTimedTaskInfoUI);
                removedTimedTaskInfoUI.QueueFree();
                timedTaskCellInfoUis.Remove(timedTaskEndedEvent.location);
            });
        }
        else
        {
            GD.PushError("how did this happen");
        }
    }

    private string GetEndAnimationNameForJob(TimedTaskEndedEvent timedTaskEndedEvent)
    {
        switch (timedTaskEndedEvent.timedTask)
        {
            case AutoUpgradeTimedTaskState autoUpgradeTimedTask:
                return "auto_upgrade";
            default:
                return "job_complete";
        }
    }
}


