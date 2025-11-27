using Godot;

public partial class TimedJobCellInfoUI : Control
{
    [Export]
    private Label workerCountLabel;

    [Export]
    private Control alert;

    public void UpdateWorkerCountLabel(int count, int capacity)
    {
        workerCountLabel.Text = $"{count}/{capacity}";
        alert.Visible = count <= 0;
    }
}

