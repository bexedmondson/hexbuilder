using Godot;

[GlobalClass][Tool]
public partial class AdjacencyConfig : Resource
{
    [Export]
    public CustomTileData requiredTile { get; private set; } = new();

    [Export]
    public int distance { get; private set; }

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> currencyEffect { get; private set; } = new();
}