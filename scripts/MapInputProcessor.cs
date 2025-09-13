using Godot;

public partial class MapInputProcessor : Node2D
{
    [Export]
    private LockedCellPopup lockedCellPopup;

    [Export]
    private UnlockedCellPopup unlockedCellPopup;

    private MapController mapController;

    public override void _Ready()
    {
        base._Ready();
        mapController = InjectionManager.Get<MapController>();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton
            && eventMouseButton.ButtonIndex == MouseButton.Left
            && !eventMouseButton.Pressed)
        {
            GD.Print(eventMouseButton.AsText());

            var cell = mapController.GetCellUnderMouse();
            var cellStatus = mapController.GetCellStatus(cell);
            
            if (cellStatus == CellStatus.Hidden)
                return;

            if (cellStatus == CellStatus.Locked)
            {
                lockedCellPopup.SetCell(cell);
                lockedCellPopup.SetVisible(true);
                unlockedCellPopup.SetVisible(false);
            }
            else
            {
                unlockedCellPopup.SetCell(cell);
                unlockedCellPopup.SetVisible(true);
                lockedCellPopup.SetVisible(false);
            }
        }
    }
}