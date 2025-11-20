using System;
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
        residenceLabel.Text = $"{houseData.location}: {houseData.occupants.Length}/{houseData.capacity}";
    }

    public void OnGoToButton()
    {
        InjectionManager.Get<MapCameraController>().FlyTo(houseData.GetCentreWorldPosition());
        onGoToButtonAction?.Invoke();
    }
}
