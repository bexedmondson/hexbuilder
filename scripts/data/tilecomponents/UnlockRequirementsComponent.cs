using Godot;

[GlobalClass][Tool]
public partial class UnlockRequirementsComponent : AbstractTileDataComponent
{
    [Export]
    //all requirements must be satisfied - AnyOf unlock requirement to be implemented when needed
    public Godot.Collections.Array<Requirement> requirements;
}
