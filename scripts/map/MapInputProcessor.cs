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
        
        if (cellStatus == CellStatus.Hidden)
            return;

        if (cellStatus == CellStatus.Locked)
        {
            unlockedCellPopup.Close();
            mapHighlightController.OnSelectCell(cell);
            lockedCellPopup.ShowForCell(cell);
        }
        else //TODO handle what to do when in unlocking state
        {
            lockedCellPopup.Close();
            mapHighlightController.OnSelectCell(cell);
            
            mapCameraController ??= InjectionManager.Get<MapCameraController>();
            var offset = mapCameraController.Zoom.Length() == 0 ? 0 : Mathf.RoundToInt(500 / mapCameraController.Zoom.Length());
            mapCameraController.FlyToCellWithOffset(cell, new Vector2I(offset, 0), 0.3f);
            
            unlockedCellPopup.ShowForCell(mapController.BaseMapLayer, cell);
        }
    }
}