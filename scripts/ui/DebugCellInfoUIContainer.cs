using System.Collections.Generic;
using Godot;

public partial class DebugCellInfoUIContainer : Control
{
    [Export]
    private PackedScene debugCellInfoUIScene;
    
    private MapController mapController;

    private Dictionary<Vector2I, DebugCellInfoUI> infoUIs = new();

    private bool isOn = false;

    public override void _Ready()
    {
        base._Ready();
        InjectionManager.Get<EventDispatcher>().Add<MapUpdatedEvent>(OnMapUpdated);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("debug_toggle"))
            Toggle();
    }

    private void OnMapUpdated(MapUpdatedEvent mapUpdatedEvent)
    {
        mapController ??= InjectionManager.Get<MapController>();

        var mapLayer = mapController.BaseMapLayer;
        
        foreach (var usedCell in mapLayer.GetUsedCells())
        {
            if (infoUIs.TryGetValue(usedCell, out var infoUI))
            {
                infoUI.Update(mapLayer.GetCellCustomData(usedCell));
            }
            else
            {
                var newInfoUI = debugCellInfoUIScene.Instantiate<DebugCellInfoUI>();
                newInfoUI.Update(mapLayer.GetCellCustomData(usedCell));
                newInfoUI.GlobalPosition = mapLayer.ToGlobal(mapLayer.MapToLocal(usedCell));
                newInfoUI.Size = Vector2I.Zero; //for some incomprehensible reason this keeps being given a 40x40 size. resetting here.
                newInfoUI.Visible = isOn;
                infoUIs[usedCell] = newInfoUI;
                this.AddChild(newInfoUI);
            }
        }
    }

    private void Toggle()
    {
        isOn = !isOn;
        
        foreach (var infoUI in infoUIs)
        {
            infoUI.Value.Visible = isOn;
        }
    }
}

