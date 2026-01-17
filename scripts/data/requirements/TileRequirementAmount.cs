using Godot;

[GlobalClass][Tool]
public partial class TileRequirementAmount : Resource
{
    [Export]
    public CustomTileData tile;

    [Export]
    public int amount;

    [Export]
    public Comparison comparison = Comparison.LessThan;
}
