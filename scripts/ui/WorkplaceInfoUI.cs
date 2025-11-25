using System;
using System.Text;
using Godot;

public partial class WorkplaceInfoUI : Control
{
    [Export]
    private Label workplaceInfoLabel;

    [Export]
    private TextureRect workplaceIcon;

    [Export]
    private Label workerNamesLabel;

    [Export]
    private Label workerCountLabel;
    
    [Export]
    private Button minusButton;
    
    [Export]
    private Button plusButton;

    private WorkplaceManager workplaceManager;

    private WorkplaceData workplaceData;
    private Action onGoToButtonAction;

    public void SetWorkplaceInfo(WorkplaceData workplaceData, Action onGoToButtonAction)
    {
        this.workplaceData = workplaceData;
        this.onGoToButtonAction = onGoToButtonAction;
        workplaceInfoLabel.Text = $"{workplaceData.location} {workplaceData.name}";

        workplaceIcon.Texture = workplaceData.iconTexture;
        
        UpdateWorkerCountUI();
    }

    private void UpdateWorkerCountUI()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var worker in workplaceData.workers)
        {
            sb.Append($"{worker.Name} ");
        }
        workerNamesLabel.Text = sb.ToString();
        
        int current = workplaceData.workers.Length;
        int max = workplaceData.capacity;

        minusButton.Disabled = current <= 0;
        plusButton.Disabled = current >= max;
        
        workerCountLabel.Text = $"{current}/{max}";
    }

    public void OnGoToButton()
    {
        InjectionManager.Get<MapCameraController>().FlyTo(workplaceData.GetCentreWorldPosition());
        onGoToButtonAction?.Invoke();
    }

    public void OnMinusButton()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        workplaceManager.TryRemoveResidentFromWorkplace(workplaceData);
        UpdateWorkerCountUI();
    }

    public void OnPlusButton()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        workplaceManager.TryAssignResidentToWorkplace(workplaceData);
        UpdateWorkerCountUI();
    }
}
