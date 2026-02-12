using Godot;

[GlobalClass]
public partial class CurrencyDeltaTileAction : AbstractTileAction
{
    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> currencyDelta { get; private set; } = new();
}
