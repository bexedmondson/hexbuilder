using Godot;

public partial class ResidentInfoPopup : Popup
{
    [Export]
    private Control residentInfoContainer;

    [Export]
    private PackedScene residentInfoUIScene;
    
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
            var residentInfoUI = residentInfoUIScene.Instantiate<ResidentInfoUI>();
            residentInfoUI.SetResident(residentData);
            residentInfoContainer.AddChild(residentInfoUI);
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
