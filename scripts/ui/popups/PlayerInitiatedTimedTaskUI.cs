using System;
using Godot;

public partial class PlayerInitiatedTimedTaskUI : Control
{
    [Export]
    private Label taskNameLabel;
    
    [Export]
    private Button goButton;

    [Export]
    private Label turnCount;

    [Export]
    private AbstractTimedTaskCompleteActionUI changeTileActionUI;

    [Export]
    private AbstractTimedTaskCompleteActionUI currencyDeltaActionUI;

    public Action OnTaskStarted;

    private PlayerInitiatedTimedTaskConfig config;
    private Vector2I cell;
    
    public void Setup(PlayerInitiatedTimedTaskConfig config, Vector2I cell)
    {
        this.config = config;
        this.cell = cell;
        taskNameLabel.Text = config.jobName;
        goButton.Text = $"x{config.workersNeeded}";
        
        var residentManager = InjectionManager.Get<ResidentManager>();
        bool notEnoughWorkers = residentManager.GetNotBusyResidentCount() < config.workersNeeded;
        goButton.Disabled = notEnoughWorkers;

        turnCount.Text = config.turnDuration.ToString();

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

    public void OnGoButton()
    {
        var timedTaskManager = InjectionManager.Get<TimedTaskManager>();
        var task = new ConfigTimedTaskState(cell, config);
        
        for (int i = 0; i < task.workerCountRequirement; i++)
        {
            timedTaskManager.TryAssignResidentToTimedTask(task);
        }
        timedTaskManager.AddNewTimedTask(task);
        
        //TODO close popup, change tile state to...something that isn't unlocked, i guess. unlocking maybe?
        OnTaskStarted?.Invoke();
    }
}
