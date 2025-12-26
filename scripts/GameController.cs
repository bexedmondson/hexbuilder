using System.Collections.Generic;
using Godot;

public partial class GameController : Node2D
{
    [Export]
    private MapController mapController;

    [Export]
    private InventoryManager inventoryManager;

    private TileDatabase tileDatabase;
    private EventDispatcher eventDispatcher;
    private MapCurrencyChangeAnalyser mapCurrencyChangeAnalyser;
    private TimedJobManager timedJobManager;

    public override void _EnterTree()
    {
        base._EnterTree();

        eventDispatcher = new EventDispatcher();

        tileDatabase = InjectionManager.Get<TileDatabase>();
        tileDatabase ??= new TileDatabase();
    }

    public override void _Ready()
    {
        base._Ready();
        
        mapController.OnNewGame();
        inventoryManager.OnNewGame();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("next_turn"))
            NextTurn();
    }

    public void NextTurn()
    {
        mapCurrencyChangeAnalyser ??= InjectionManager.Get<MapCurrencyChangeAnalyser>();
        List<CurrencySum> allCurrencyChanges = mapCurrencyChangeAnalyser.GetFullCurrencyTurnDeltas();
        
        inventoryManager.OnNextTurn(allCurrencyChanges.ToArray());
        
        InjectionManager.Get<ResidentManager>().OnNextTurn();

        timedJobManager ??= InjectionManager.Get<TimedJobManager>();
        timedJobManager.OnNextTurn();
    }
}