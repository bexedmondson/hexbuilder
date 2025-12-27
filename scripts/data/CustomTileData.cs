using System;
using Godot;

[GlobalClass][Tool]
public partial class CustomTileData : Resource
{
    [Export]
    public Godot.Collections.Array<CustomTileData> canBePlacedOn;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> buildPrice;

    [Export] //all requirements must be satisfied - AnyOf unlock requirement to be implemented when needed
    public Godot.Collections.Array<UnlockRequirement> unlockRequirements;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> baseTurnCurrencyChange;

    [Export]
    public Godot.Collections.Array<AdjacencyConfig> adjacencies;

    [Export]
    public int residentCapacity;

    public bool IsResidence => residentCapacity > 0;
    
    [Export]
    public int workerCapacity;
    
    public bool IsWorkplace => workerCapacity > 0;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> storageCapacity;

    [Export]
    public Godot.Collections.Array<AbstractTileDataComponent> components = new();
    
    protected System.Collections.Generic.Dictionary<Type, AbstractTileDataComponent> componentDict = null;
    
	public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : AbstractTileDataComponent
	{
		if (componentDict == null)
		{
			componentDict = new System.Collections.Generic.Dictionary<Type, AbstractTileDataComponent>();
			
			foreach (var arrayComponent in components)
				componentDict[arrayComponent.GetType()] = arrayComponent;
		}
		
		if (componentDict.ContainsKey(typeof(TComponent)))
		{
			component = componentDict[typeof(TComponent)] as TComponent;
			return true;
		}
		
		foreach (var kvp in componentDict)
		{
			if (kvp.Key.IsInstanceOfType(typeof(TComponent))) //handling component type inheritance
			{
				component = kvp.Value as TComponent;
				return true;
			}
		}

		component = null;
		return false;
	}

    public bool TryGetAdjacencyEffectFromTileData(CustomTileData otherTile, out CurrencySum effect)
    {
        effect = new();
        if (adjacencies == null || adjacencies.Count == 0)
            return false;
        
        foreach (var adjacency in adjacencies)
        {
            if (adjacency.requiredTile == otherTile)
                effect.Add(adjacency.currencyEffect);
        }

        return effect.Count != 0;
    }

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