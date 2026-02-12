using Godot;

public partial class PlayerInitiatedTimedTaskUI : Control
{
    [Export]
    private Label taskNameLabel;
    
    [Export]
    private Button goButton;

    [Export]
    private AbstractTimedTaskCompleteActionUI changeTileActionUI;

    [Export]
    private AbstractTimedTaskCompleteActionUI currencyDeltaActionUI;
    
    public void Setup(PlayerInitiatedTimedTaskConfig config, Vector2I cell)
    {
        taskNameLabel.Text = config.jobName;
        goButton.Text = $"x{config.workersNeeded}";
        
        var residentManager = InjectionManager.Get<ResidentManager>();
        bool notEnoughWorkers = residentManager.GetNotBusyResidentCount() < config.workersNeeded;
        goButton.Disabled = notEnoughWorkers;

        changeTileActionUI.Visible = false;
        currencyDeltaActionUI.Visible = false;
        
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
