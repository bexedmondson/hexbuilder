using Godot;

public partial class CurrencyDeltaTimedTaskCompleteActionUI : AbstractTimedTaskCompleteActionUI
{
    [Export]
    private CurrencyDisplay delta;

    public override void Setup(AbstractTileAction completeAction, Vector2I cell)
    {
        var action = completeAction as CurrencyDeltaTileAction;
        delta.DisplayCurrencyAmount(action.currencyDelta);
    }
}
