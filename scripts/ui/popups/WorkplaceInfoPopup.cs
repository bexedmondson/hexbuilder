using Godot;

public partial class WorkplaceInfoPopup : Popup
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

    public override void Close()
    {
        base.Close();
        for (int i = workplaceInfoContainer.GetChildCount() - 1; i >= 0; i--)
        {
            var info = workplaceInfoContainer.GetChild(i);
            workplaceInfoContainer.RemoveChild(info);
            info.QueueFree();
        }
    }
}
