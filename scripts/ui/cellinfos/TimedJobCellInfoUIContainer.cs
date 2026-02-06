using System.Collections.Generic;
using Godot;

public partial class TimedJobCellInfoUIContainer : Control
{
    [Export]
    private TileMapLayer baseTileMapLayer;

    [Export]
    private PackedScene cellInfoUIScene;

    private Dictionary<Vector2I, TimedJobCellInfoUI> timedJobCellInfoUis = new();

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Get<EventDispatcher>().Add<TimedJobStartedEvent>(OnTimedJobStarted);
        InjectionManager.Get<EventDispatcher>().Add<TimedJobUpdatedEvent>(OnTimedJobUpdated);
        InjectionManager.Get<EventDispatcher>().Add<TimedJobEndedEvent>(OnTimedJobEnded);
    }

    private void OnTimedJobStarted(TimedJobStartedEvent timedJobStartedEvent)
    {
        if (timedJobCellInfoUis.ContainsKey(timedJobStartedEvent.location))
        {
            GD.PushError("how did this happen");
        }

        var newTimedJobCellInfoUI = cellInfoUIScene.Instantiate<TimedJobCellInfoUI>();
        newTimedJobCellInfoUI.UpdateWorkerCountLabel(timedJobStartedEvent.workerCount, timedJobStartedEvent.capacity);
        newTimedJobCellInfoUI.UpdateTurnCountLabel(timedJobStartedEvent.timedJob.turnDuration);
        newTimedJobCellInfoUI.GlobalPosition = baseTileMapLayer.ToGlobal(baseTileMapLayer.MapToLocal(timedJobStartedEvent.location));
        timedJobCellInfoUis[timedJobStartedEvent.location] = newTimedJobCellInfoUI;
        this.AddChild(newTimedJobCellInfoUI);
    }

    private void OnTimedJobUpdated(TimedJobUpdatedEvent timedJobUpdatedEvent)
    {
        if (timedJobCellInfoUis.TryGetValue(timedJobUpdatedEvent.location, out var updatedJobUI))
        {
            updatedJobUI.UpdateTurnCountLabel(timedJobUpdatedEvent.timedJob.turnDuration - timedJobUpdatedEvent.timedJob.turnsElapsed);
            updatedJobUI.UpdateWorkerCountLabel(timedJobUpdatedEvent.timedJob.workerCount, timedJobUpdatedEvent.timedJob.workerCountRequirement);
            updatedJobUI.DoTurnAnim();
        }
    }
    
    private void OnTimedJobEnded(TimedJobEndedEvent timedJobEndedEvent)
    {
        if (timedJobCellInfoUis.TryGetValue(timedJobEndedEvent.location, out var removedTimedJobInfoUI))
        {
            removedTimedJobInfoUI.AnimateOut(
                GetEndAnimationNameForJob(timedJobEndedEvent),
                () =>
            {
                this.RemoveChild(removedTimedJobInfoUI);
                removedTimedJobInfoUI.QueueFree();
                timedJobCellInfoUis.Remove(timedJobEndedEvent.location);
            });
        }
        else
        {
            GD.PushError("how did this happen");
        }
    }

    private string GetEndAnimationNameForJob(TimedJobEndedEvent timedJobEndedEvent)
    {
        switch (timedJobEndedEvent.timedJob)
        {
            case AutoUpgradeTimedJobState autoUpgradeTimedJob:
                return "auto_upgrade";
            default:
                return "job_complete";
        }
    }
}


