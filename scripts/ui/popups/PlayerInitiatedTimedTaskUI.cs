using Godot;

public partial class PlayerInitiatedTimedTaskUI : Control
{
    [Export]
    private Label taskNameLabel;
    
    [Export]
    private Label workerCountLabel;
    
    [Export]
    private Button goButton;

    [Export]
    private AbstractTimedTaskCompleteActionUI changeTileActionUI;

    [Export]
    private AbstractTimedTaskCompleteActionUI currencyDeltaActionUI; 
    
    public void Setup(PlayerInitiatedTimedTaskConfig config, Vector2I cell)
    {
        taskNameLabel.Text = config.jobName;
        workerCountLabel.Text = $"x{config.workersNeeded}";
        
        var residentManager = InjectionManager.Get<ResidentManager>();
        int availableResidents = residentManager.GetNotBusyResidentCount();
        goButton.Disabled = config.workersNeeded < availableResidents;
        
        foreach (var configCompleteAction in config.completeActions)
        {
            switch (configCompleteAction)
            {
                case ChangeTileTypeTileAction _:
                    changeTileActionUI.Visible = true;
                    changeTileActionUI.Setup(configCompleteAction, cell);
                    break;
                case CurrencyDeltaTileAction _:
                    currencyDeltaActionUI.Visible = true;
                    currencyDeltaActionUI.Setup(configCompleteAction, cell);
                    break;
            }
        }
    }
}
