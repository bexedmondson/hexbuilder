using Godot;

public partial class ResidentInfoPopup : Popup
{
    [Export]
    private Control residentInfoContainer;
    
    private ResidentManager residentManager;

    public override void _Ready()
    {
        base._Ready();
        this.Visible = false;
    }

    public void ShowPopup()
    {
        residentManager ??= InjectionManager.Get<ResidentManager>();
        
        foreach (var residentData in residentManager.AllResidents)
        {
            Label residentLabel = new Label();
            residentLabel.Text = $"{residentData.Name}{(residentData.HasHouse ? string.Empty : " - no house! :(")} - {(residentData.HasWorkplace ? $"is employed at {residentData.GetWorkplaceName()}" : "is not employed")}";
                
            residentInfoContainer.AddChild(residentLabel);
        }

        this.Visible = true;
    }

    public override void Close()
    {
        base.Close();
        for (int i = residentInfoContainer.GetChildCount() - 1; i >= 0; i--)
        {
            var info = residentInfoContainer.GetChild(i);
            residentInfoContainer.RemoveChild(info);
            info.QueueFree();
        }
    }
}
