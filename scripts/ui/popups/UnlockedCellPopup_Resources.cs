using Godot;

public partial class UnlockedCellPopup_Resources : Control
{
    [Export]
    private CurrencyDisplay overallEffectDisplay;

    [Export]
    private Control defaultBaseEffectContainer;

    [Export]
    private CurrencyDisplay_Strikethroughable defaultBaseEffectDisplay;

    [Export]
    private CurrencyDisplay actualBaseEffectDisplay;

    [Export]
    private Control noProductionIndicator;

    [Export]
    private Control noConsumptionIndicator;

    [Export]
    private Control maxBonusContainer;

    [Export]
    private CurrencyDisplay_Strikethroughable maxBonusCurrencyDisplay;

    [Export]
    private Control maxBonusNegationReason;

    [Export]
    private Control adjacencyEffectContainer;

    [Export]
    private PackedScene adjacencyEffectUIScene;

    private MapController mapController;
    private WorkplaceManager workplaceManager;

    private CustomTileData selectedCellTileData;
    private Vector2I selectedCell;
    private MapCurrencyChangeAnalyser currencyChangeAnalyser;

    public override void _Ready()
    {
        base._Ready();
        mapController = InjectionManager.Get<MapController>();
        workplaceManager = InjectionManager.Get<WorkplaceManager>();
        currencyChangeAnalyser = InjectionManager.Get<MapCurrencyChangeAnalyser>();
    }

    public void Setup(CustomTileData cellTileData, Vector2I cell, out bool hasEffects)
    {
        Cleanup();
        
        this.selectedCellTileData = cellTileData;
        this.selectedCell = cell;
        
        var adjacentCells = mapController.BaseMapLayer.GetSurroundingCells(selectedCell);

        hasEffects = false;

        overallEffectDisplay.DisplayCurrencyAmount(currencyChangeAnalyser.GetTotalCellCurrencyChange(cell));
        
        //if there should be a base currency change (even if it's being changed to 0 for some reason) then display it
        if (selectedCellTileData.baseTurnCurrencyChange?.Count > 0)
        {
            hasEffects = true;
            defaultBaseEffectContainer.Visible = true;
            CurrencySum defaultBaseChange = new CurrencySum(selectedCellTileData.baseTurnCurrencyChange);
            
            defaultBaseEffectDisplay.DisplayCurrencyAmount(defaultBaseChange);
            
            CurrencySum actualCurrencyChange = currencyChangeAnalyser.GetActualCellBaseCurrencyChange(selectedCell, selectedCellTileData);
            
            if (defaultBaseChange != actualCurrencyChange)
            {
                defaultBaseEffectDisplay.SetStrikethrough(true);
                actualBaseEffectDisplay.DisplayCurrencyAmount(actualCurrencyChange);
                noProductionIndicator.Visible = currencyChangeAnalyser.IsCellProductionProhibited(cell, cellTileData);
                noConsumptionIndicator.Visible = currencyChangeAnalyser.IsCellConsumptionProhibited(cell, cellTileData);
            }
            else
            {
                defaultBaseEffectDisplay.SetStrikethrough(false);
                noProductionIndicator.Visible = false;
                noConsumptionIndicator.Visible = false;
            }
        }
        else
        {
            defaultBaseEffectContainer.Visible = false;
        }

        if (selectedCellTileData.IsWorkplace)
        {
            SetupMaxBonus(selectedCellTileData, selectedCell);
        }
        else
        {
            maxBonusContainer.Visible = false;
        }

        foreach (var adjacentCell in adjacentCells)
        {
            if (mapController.GetCellStatus(adjacentCell) != CellStatus.Unlocked)
                continue;
            
            var adjacentData = mapController.BaseMapLayer.GetCellCustomData(adjacentCell);
            bool hasGivenEffect = selectedCellTileData.TryGetAdjacencyEffectFromTileData(adjacentData, out var givenEffect);
            bool hasReceivedEffect = adjacentData.TryGetAdjacencyEffectFromTileData(selectedCellTileData, out var receivedEffect);

            if (!hasGivenEffect)// && !hasReceivedEffect)
                continue;
            
            hasEffects = true;
            
            var adjacencyEffectUI = adjacencyEffectUIScene.Instantiate<UnlockedCellAdjacencyUI>();
            adjacencyEffectUI.Setup(adjacentData, cell, adjacentCell, currencyChangeAnalyser.IsCellProductionProhibited(cell, cellTileData));
            if (hasGivenEffect)
                adjacencyEffectUI.SetGivenEffects(givenEffect);
            //if (hasReceivedEffect)
            //    adjacencyEffectUI.SetReceivedEffects(receivedEffect);
            adjacencyEffectContainer.AddChild(adjacencyEffectUI);
        }
    }

    private void SetupMaxBonus(CustomTileData tileData, Vector2I cell)
    {
        bool hasMaxBonus = tileData.TryGetComponent(out MaximumWorkerProductionBonusComponent maxBonusComponent);
        workplaceManager.TryGetWorkplaceAtLocation(cell, out var workplaceState);
        maxBonusContainer.Visible = hasMaxBonus && workplaceState.workerCount >= workplaceState.capacity;
        if (!hasMaxBonus) 
            return;
        
        InjectionManager.Get<EventDispatcher>().Add<WorkplaceUpdatedEvent>(OnWorkplaceUpdated);
        maxBonusCurrencyDisplay.DisplayCurrencyAmount(maxBonusComponent.extraBaseProduction);
    }

    private void OnWorkplaceUpdated(WorkplaceUpdatedEvent e)
    {
        workplaceManager.TryGetWorkplaceAtLocation(selectedCell, out var workplaceState);
        if (!e.newOrChangedWorkplaces.Contains(workplaceState))
            return;
        
        maxBonusContainer.Visible = workplaceState.workerCount >= workplaceState.capacity;
        //don't need to do the rest of the setup - shouldn't have gotten here if no max bonus component/not a workplace
    }

    public void Cleanup()
    {
        for (int i = adjacencyEffectContainer.GetChildCount() - 1; i >= 0; i--)
        {
            adjacencyEffectContainer.GetChild(i).QueueFree();
        }

        selectedCellTileData = null;
        selectedCell = Vector2I.MinValue;
    }
}
