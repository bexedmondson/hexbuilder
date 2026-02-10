using Godot;

[GlobalClass][Tool]
public partial class MaximumWorkerProductionBonusComponent : AbstractTileDataComponent
{
    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> extraBaseProduction { get; private set; } = new();
}
