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

    private WorkplaceState workplaceState;
    private Action onGoToButtonAction;

    public void SetWorkplaceInfo(WorkplaceState workplaceState, Action onGoToButtonAction)
    {
        this.workplaceState = workplaceState;
        this.onGoToButtonAction = onGoToButtonAction;
        workplaceInfoLabel.Text = $"{workplaceState.location} {workplaceState.name}";

        workplaceIcon.Texture = workplaceState.iconTexture;
        
        UpdateWorkerCountUI();
    }

    private void UpdateWorkerCountUI()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var worker in workplaceState.workers)
        {
            sb.Append($"{worker.Name} ");
        }
        workerNamesLabel.Text = sb.ToString();
        
        int current = workplaceState.workerCount;
        int max = workplaceState.capacity;

        minusButton.Disabled = current <= 0;
        plusButton.Disabled = current >= max;
        
        workerCountLabel.Text = $"{current}/{max}";
    }

    public void OnGoToButton()
    {
        InjectionManager.Get<MapCameraController>().FlyToCell(workplaceState.location);
        onGoToButtonAction?.Invoke();
    }

    public void OnMinusButton()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        workplaceManager.TryRemoveResidentFromWorkplace(workplaceState);
        UpdateWorkerCountUI();
    }

    public void OnPlusButton()
    {
        workplaceManager ??= InjectionManager.Get<WorkplaceManager>();
        workplaceManager.TryAssignResidentToWorkplace(workplaceState);
        UpdateWorkerCountUI();
    }
}
