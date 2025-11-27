
using Godot;

public partial class ResidentInfoUI : Control
{
    [Export]
    private Label residentLabel;
    
    public void SetResident(ResidentData residentData)
    {
        residentLabel.Text = $"{residentData.Name}{(residentData.HasHouse ? string.Empty : " - no house! :(")} - {(residentData.IsBusy ? $"is employed at {residentData.GetWorkplaceName()}" : "is not employed")}";
    }
}
