using Godot;

[GlobalClass][Tool]
public partial class StorageCapacityDataComponent : AbstractTileDataComponent
{
    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> capacities;
}
