using System;
using System.Text;
using Godot;

public partial class HouseInfoUI : Control
{
    [Export]
    private Label residenceLabel;

    private HouseData houseData;
    private Action onGoToButtonAction;

    public void SetHouseInfo(HouseData houseData, Action onGoToButtonAction)
    {
        this.houseData = houseData;
        this.onGoToButtonAction = onGoToButtonAction;
        StringBuilder sb = new StringBuilder($"{houseData.location}: {houseData.occupants.Length}/{houseData.capacity}");

        foreach (var occupant in houseData.occupants)
        {
            sb.Append($" {occupant.Name}");
        }
        residenceLabel.Text = sb.ToString();
    }

    public void OnGoToButton()
    {
        InjectionManager.Get<MapCameraController>().FlyTo(houseData.GetCentreWorldPosition());
        onGoToButtonAction?.Invoke();
    }
}
