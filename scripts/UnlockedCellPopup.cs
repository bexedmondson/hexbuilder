using Godot;

public partial class UnlockedCellPopup : Control
{
    private Vector2I cell;

    public override void _Ready()
    {
        this.SetVisible(false);
    }
    
    public void SetCell(Vector2I setCell)
    {
        cell = setCell;
    }

    public void Close()
    {
        this.SetVisible(false);
    }
}