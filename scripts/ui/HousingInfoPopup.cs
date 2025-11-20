
using System.Text;
using Godot;

public partial class HousingInfoPopup : Control
{
    [Export]
    private Control houseInfoContainer;
    
    [Export]
    private PackedScene houseInfoScene;
    
    private HousingManager housingManager;

    public override void _Ready()
    {
        base._Ready();
        this.Visible = false;
    }

    public void ShowPopup()
    {
        housingManager ??= InjectionManager.Get<HousingManager>();

        foreach (var houseData in housingManager.AllHouseDatas)
        {
            var houseInfoUI = houseInfoScene.Instantiate<HouseInfoUI>();
            houseInfoUI.SetHouseInfo(houseData, Close);
            houseInfoContainer.AddChild(houseInfoUI);
        }

        this.Visible = true;
    }

    public void Close()
    {
        this.Visible = false;
    }
}
