using Godot;

[GlobalClass]
public partial class CurrencyDeltaTileAction : AbstractTileAction
{
    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> currencyDelta { get; private set; } = new();
    
    public override void DoAction(Vector2I _)
    {
        InjectionManager.Get<InventoryManager>().AwardCurrency(new CurrencySum(currencyDelta));
    }
}
