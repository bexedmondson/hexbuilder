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

    [Export]
    public Godot.Collections.Array<AdjacencyConfig> adjacencies;

    [Export] //all requirements must be satisfied - AnyOf unlock requirement to be implemented when needed
    public Godot.Collections.Array<UnlockRequirement> unlockRequirements;

    [Export]
    public int residentCapacity;

    public bool IsUnlocked()
    {
        if (unlockRequirements == null)
            return true;
        
        foreach (var unlockRequirement in unlockRequirements)
        {
            if (!unlockRequirement.IsSatisfied())
                return false;
        }
        return true;
    }

    public string GetFileName()
    {
        return ResourcePath.GetFile().TrimSuffix("." + ResourcePath.GetFile().GetExtension());
    }
}