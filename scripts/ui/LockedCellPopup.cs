using Godot;

public partial class LockedCellPopup : Popup
{
    [Export]
    private Label workerCountLabel;

    [Export]
    private Button confirmButton;

    private MapController mapController;
    private ResidentManager residentManager;
    
    private Vector2I cell;

    public override void _Ready()
    {
        this.SetVisible(false);
        mapController = InjectionManager.Get<MapController>();
        residentManager = InjectionManager.Get<ResidentManager>();
    }

    public void ShowForCell(Vector2I setCell)
    {
        cell = setCell;
        int requiredWorkerCount = mapController.GetCellUnlockCost(cell);
        int availableResidents = residentManager.GetNotBusyResidentCount();
        
        workerCountLabel.Text = $"x{requiredWorkerCount}";

        bool hasEnoughAvailableWorkers = requiredWorkerCount <= availableResidents;
        confirmButton.Disabled = !hasEnoughAvailableWorkers;
        
        this.SetVisible(true);
        InjectionManager.Get<MapHighlightController>().OnSelectTile(cell);
    }

    public override void Confirm()
    {
        int requiredWorkerCount = mapController.GetCellUnlockCost(cell);
        int availableResidents = residentManager.GetNotBusyResidentCount();
        if (requiredWorkerCount > availableResidents)
            return;

        mapController.OnCellUnlockInitiated(cell);
        Close();
    }

    public override void Close()
    {
        base.Close();
        this.SetVisible(false);
        InjectionManager.Get<MapHighlightController>().Clear();
    }
}