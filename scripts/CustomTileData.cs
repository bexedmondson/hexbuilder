using Godot;

[GlobalClass][Tool]
public partial class CustomTileData : Resource
{
    [Export]
    public Godot.Collections.Array<CustomTileData> canBePlacedOn;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> price;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> baseTurnCurrencyChange;
}