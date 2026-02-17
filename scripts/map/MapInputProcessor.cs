using Godot;

public partial class MapInputProcessor : Node2D
{
    //relocate in future - works for now though
    [Export]
    private LockedCellPopup lockedCellPopup;

    [Export]
    private UnlockedCellPopup unlockedCellPopup;

    [Export]
    private float dragSquareDistanceMinThreshold = 1000f;

    private MapController mapController;
    private MapHighlightController mapHighlightController;
    private MapCameraController mapCameraController;

    private Vector2 pressStartPosition;

    public override void _Ready()
    {
        base._Ready();
        mapController = InjectionManager.Get<MapController>();
        mapHighlightController = InjectionManager.Get<MapHighlightController>();
        mapCameraController = InjectionManager.Get<MapCameraController>();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //if (@event is not InputEventMouseMotion)
            //GD.Print($"{System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")} {@event}");

        if (@event is not InputEventScreenTouch eventMouseButton)
            return;

        if (eventMouseButton.Pressed)
        {
            pressStartPosition = eventMouseButton.Position;
            return;
        }
        
        if ((eventMouseButton.Position - pressStartPosition).LengthSquared() > dragSquareDistanceMinThreshold)
            return;
        
        var cell = mapController.GetCellUnderMouse();
        var cellStatus = mapController.GetCellStatus(cell);

        switch (cellStatus)
        {
            case CellStatus.Hidden:
            case CellStatus.Busy: //TODO do something better here?
                return; 
            case CellStatus.Locked:
                unlockedCellPopup.Close();
                mapHighlightController.OnSelectCell(cell);
                lockedCellPopup.ShowForCell(cell);
                break;
            case CellStatus.Unlocked:
                lockedCellPopup.Close();
                mapHighlightController.OnSelectCell(cell);
                
                mapCameraController ??= InjectionManager.Get<MapCameraController>();
                var offset = mapCameraController.Zoom.Length() == 0 ? 0 : Mathf.RoundToInt(500 / mapCameraController.Zoom.Length());
                mapCameraController.FlyToCellWithOffset(cell, new Vector2I(offset, 0), 0.3f);
                mapCameraController.ResetZoom(0.5f);
                
                unlockedCellPopup.ShowForCell(mapController.BaseMapLayer, cell);
                break;
        }
    }
}