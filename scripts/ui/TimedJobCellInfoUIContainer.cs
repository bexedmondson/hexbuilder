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
        newTimedJobCellInfoUI.GlobalPosition = baseTileMapLayer.ToGlobal(baseTileMapLayer.MapToLocal(timedJobStartedEvent.location));
        //newTimedJobCellInfoUI.Size = Vector2I.Zero; //for some incomprehensible reason this keeps being given a 40x40 size. resetting here.
        timedJobCellInfoUis[timedJobStartedEvent.location] = newTimedJobCellInfoUI;
        this.AddChild(newTimedJobCellInfoUI);
    }
    
    private void OnTimedJobEnded(TimedJobEndedEvent timedJobEndedEvent)
    {
        if (timedJobCellInfoUis.TryGetValue(timedJobEndedEvent.location, out var removedTimedJobInfoUI))
        {
            removedTimedJobInfoUI.AnimateOut(() =>
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
}


