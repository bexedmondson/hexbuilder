using Godot;

[GlobalClass][Tool]
public partial class AdjacencyRequirement : DataRequirement
{
    [Export]
    public CustomTileData neighbourTile;
    
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new AdjacencyRequirementProcessor(this);
    }
}