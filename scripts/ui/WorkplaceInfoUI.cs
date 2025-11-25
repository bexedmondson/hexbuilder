using System;
using System.Text;
using Godot;

public partial class WorkplaceInfoUI : Control
{
    [Export]
    private Label workplaceLabel;

    [Export]
    private TextureRect workplaceIcon;

    private WorkplaceData workplaceData;
    private Action onGoToButtonAction;

    public void SetWorkplaceInfo(WorkplaceData workplaceData, Action onGoToButtonAction)
    {
        this.workplaceData = workplaceData;
        this.onGoToButtonAction = onGoToButtonAction;
        StringBuilder sb = new StringBuilder($"{workplaceData.location} {workplaceData.name}: {workplaceData.workers.Length}/{workplaceData.capacity}");

        foreach (var worker in workplaceData.workers)
        {
            sb.Append($" {worker.Name}");
        }
        workplaceLabel.Text = sb.ToString();
        
        //workplaceIcon.Texture = InjectionManager.Get<TileDatabase>().GetTileTexture()
    }

    public void OnGoToButton()
    {
        InjectionManager.Get<MapCameraController>().FlyTo(workplaceData.GetCentreWorldPosition());
        onGoToButtonAction?.Invoke();
    }
}
