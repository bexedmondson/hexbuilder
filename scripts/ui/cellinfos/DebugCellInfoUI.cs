using Godot;

public partial class DebugCellInfoUI : Control
{
    [Export]
    private Label label;

    private MapController mapController;

    public void Update(CustomTileData tileData, Vector2I cell)
    {
        mapController ??= InjectionManager.Get<MapController>();
        label.Text = $"{(tileData == null ? "null" : tileData.GetFileName())} {cell.ToString()}\n{mapController.GetCellStatus(cell)}";
    }
}
