
using Godot;

public partial class ResidentInfoUI : Control
{
    [Export]
    private Label residentLabel;

    [Export]
    private TextureRect happinessIcon;

    [Export]
    private Label noHouseLabel; 

    [Export]
    private Label employmentLabel;
    
    public void SetResident(ResidentState residentState)
    {
        residentLabel.Text = residentState.Name;
        happinessIcon.Texture = InjectionManager.Get<IconMapper>().happinessMap[residentState.happiness];
        noHouseLabel.Visible = !residentState.HasHouse;
        employmentLabel.Text = residentState.IsBusy ? $"is employed at {residentState.GetWorkplaceOrJobName()}" : "is not employed";
    }
}
