using Godot;

[GlobalClass][Tool]
public partial class AdjacencyConfig : Resource
{
    [Export]
    public CustomTileData requiredTile;

    [Export]
    public int distance;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> currencyEffect;
}