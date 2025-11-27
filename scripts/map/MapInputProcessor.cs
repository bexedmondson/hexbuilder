using Godot;

public partial class MapInputProcessor : Node2D
{
    //relocate in future - works for now though
    [Export]
    private LockedCellPopup lockedCellPopup;

    [Export]
    private UnlockedCellPopup unlockedCellPopup;

    private MapController mapController;
    private MapHighlightController mapHighlightController;

    public override void _Ready()
    {
        base._Ready();
        mapController = InjectionManager.Get<MapController>();
        mapHighlightController = InjectionManager.Get<MapHighlightController>();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton
            && eventMouseButton.ButtonIndex == MouseButton.Left
            && !eventMouseButton.Pressed)
        {
            var cell = mapController.GetCellUnderMouse();
            var cellStatus = mapController.GetCellStatus(cell);
            
            if (cellStatus == CellStatus.Hidden)
                return;
            
            mapHighlightController.OnSelectTile(cell);

            if (cellStatus == CellStatus.Locked)
            {
                lockedCellPopup.ShowForCell(cell);
                unlockedCellPopup.Close();
            }
            else //TODO handle what to do when in unlocking state
            {
                unlockedCellPopup.ShowForCell(mapController.BaseMapLayer, cell);
                lockedCellPopup.Close();
            }
        }
    }
}