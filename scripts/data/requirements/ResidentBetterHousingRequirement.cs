using Godot;
using Godot.Collections;

[GlobalClass][Tool]
public partial class ResidentBetterHousingRequirement : DataRequirement
{
    [Export]
    public Array<CustomTileData> unacceptableHousing;
    
    public override DataRequirementProcessor GetDataRequirementProcessor()
    {
        return new ResidentBetterHousingRequirementProcessor(this);
    }
}
