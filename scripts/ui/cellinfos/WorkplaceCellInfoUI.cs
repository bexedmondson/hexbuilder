using Godot;

public partial class WorkplaceCellInfoUI : Control
{
    [Export]
    private Label workerCountLabel;

    [Export]
    private Control alert;

    [Export]
    private ShineTweener shineTweener;

    public void UpdateWorkerCountLabel(int count, int capacity)
    {
        workerCountLabel.Text = $"{count}/{capacity}";
        alert.Visible = count <= 0;
    }

    public void SetWorkerMaxBonusEffect(bool active)
    {
        shineTweener.SetShining(active);
    }
}
