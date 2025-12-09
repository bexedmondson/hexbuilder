using Godot;

public partial class DebugCellInfoUI : Control
{
    [Export]
    private Label label;

    public void Update(CustomTileData tileData)
    {
        label.Text = tileData == null ? "null" : tileData.GetFileName();
    }
}
