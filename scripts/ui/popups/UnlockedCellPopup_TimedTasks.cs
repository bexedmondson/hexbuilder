using Godot;

public partial class UnlockedCellPopup_TimedTasks : Control
{
    [Export]
    private Control timedTasksUIContainer;

    [Export]
    private PackedScene timedTaskScene;
    
    private MapController mapController;

    private UnlockedCellPopup popup;
    private CustomTileData selectedCellTileData;
    private Vector2I selectedCell;

    public override void _Ready()
    {
        base._Ready();
        mapController = InjectionManager.Get<MapController>();
    }

    public void Setup(UnlockedCellPopup popup, CustomTileData cellTileData, Vector2I cell)
    {
        this.popup = popup;
        Cleanup();

        if (!cellTileData.TryGetComponent(out TileTimedTasksComponent timedTasksComponent))
            return;
        
        this.selectedCellTileData = cellTileData;
        this.selectedCell = cell;
        
        foreach (var timedTaskConfig in timedTasksComponent.tileTimedTasks)
        {
            if (timedTaskConfig is not PlayerInitiatedTimedTaskConfig playerInitiatedTimedTaskConfig)
                continue;

            var timedTaskUI = timedTaskScene.Instantiate<PlayerInitiatedTimedTaskUI>();
            timedTaskUI.Setup(playerInitiatedTimedTaskConfig, cell);
            timedTasksUIContainer.AddChild(timedTaskUI);
            timedTaskUI.OnTaskStarted += popup.Close;
        }
    }
    
    public void Cleanup()
    {
        for (int i = timedTasksUIContainer.GetChildCount() - 1; i >= 0; i--)
        {
            timedTasksUIContainer.GetChild(i).QueueFree();
        }

        selectedCellTileData = null;
        selectedCell = Vector2I.MinValue;
    }
}
