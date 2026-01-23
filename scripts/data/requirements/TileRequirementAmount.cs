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

    public bool IsPass(int amountToCompare)
    {
        return amountToCompare.IsPass(comparison, amount);
    }
}
