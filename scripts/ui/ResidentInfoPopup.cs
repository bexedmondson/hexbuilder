using Godot;

public partial class ResidentInfoPopup : Control
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
            residentLabel.Text = $"{residentData.Name}{(residentData.HasHouse ? string.Empty : " - no house! :(")}";
                
            residentInfoContainer.AddChild(residentLabel);
        }

        this.Visible = true;
    }

    public void Close()
    {
        for (int i = residentInfoContainer.GetChildCount() - 1; i >= 0; i--)
        {
            residentInfoContainer.RemoveChild(residentInfoContainer.GetChild(i));
        }
        this.Visible = false;
    }
}
