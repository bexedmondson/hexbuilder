
using Godot;

public partial class ResidentInfoUI : Control
{
    [Export]
    private Label residentLabel;
    
    public void SetResident(ResidentState residentState)
    {
        residentLabel.Text = $"{residentState.Name}{(residentState.HasHouse ? string.Empty : " - no house! :(")} - {(residentState.IsBusy ? $"is employed at {residentState.GetWorkplaceOrJobName()}" : "is not employed")}";
    }
}
