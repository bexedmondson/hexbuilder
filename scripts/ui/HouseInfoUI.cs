using System;
using System.Text;
using Godot;

public partial class HouseInfoUI : Control
{
    [Export]
    private Label residenceLabel;

    private HouseState houseState;
    private Action onGoToButtonAction;

    public void SetHouseInfo(HouseState houseState, Action onGoToButtonAction)
    {
        this.houseState = houseState;
        this.onGoToButtonAction = onGoToButtonAction;
        StringBuilder sb = new StringBuilder($"{houseState.location}: {houseState.occupants.Length}/{houseState.capacity}");

        foreach (var occupant in houseState.occupants)
        {
            sb.Append($" {occupant.Name}");
        }
        residenceLabel.Text = sb.ToString();
    }

    public void OnGoToButton()
    {
        InjectionManager.Get<MapCameraController>().FlyTo(houseState.GetCentreWorldPosition());
        onGoToButtonAction?.Invoke();
    }
}
