using Godot;

public partial class WorkplaceInfoPopup : Control
{
    [Export]
    private Control workplaceInfoContainer;
    
    [Export]
    private PackedScene workplaceInfoScene;
    
    private WorkplaceManager workplaceManager;

    public override void _Ready()
    {
        base._Ready();
        this.Visible = false;
    }

    public void ShowPopup()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        
        foreach (var workplaceData in workplaceManager.AllWorkplaceDatas)
        {
            var workplaceInfoUI = workplaceInfoScene.Instantiate<WorkplaceInfoUI>();
            workplaceInfoUI.SetWorkplaceInfo(workplaceData, Close);
            workplaceInfoContainer.AddChild(workplaceInfoUI);
        }

        this.Visible = true;
    }

    public void Close()
    {
        for (int i = workplaceInfoContainer.GetChildCount() - 1; i >= 0; i--)
        {
            workplaceInfoContainer.RemoveChild(workplaceInfoContainer.GetChild(i));
        }
        this.Visible = false;
    }
}
