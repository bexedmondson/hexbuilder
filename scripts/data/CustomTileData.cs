using System;
using Godot;

[GlobalClass][Tool]
public partial class CustomTileData : Resource
{
    [Export]
    public Godot.Collections.Array<CustomTileData> canBePlacedOn;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> buildPrice;

    [Export]
    public Godot.Collections.Dictionary<CurrencyType, int> baseTurnCurrencyChange;

    [Export]
    public Godot.Collections.Array<AdjacencyConfig> adjacencies;

    [Export]
    public Godot.Collections.Array<AbstractTileDataComponent> components = new();
    
    public bool IsResidence => TryGetComponent(out ResidentCapacityComponent c) && c.capacity > 0;
    public bool IsWorkplace => TryGetComponent(out WorkerCapacityComponent c) && c.capacity > 0;
    
    protected System.Collections.Generic.Dictionary<Type, AbstractTileDataComponent> componentDict = null;
    
	public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : AbstractTileDataComponent
	{
		if (componentDict == null)
		{
			componentDict = new System.Collections.Generic.Dictionary<Type, AbstractTileDataComponent>();
			
			foreach (var arrayComponent in components)
			{
				if (arrayComponent == null)
				{
					GD.PrintErr($"Null tile data component found in {this.GetFileName()}!");
					continue;
				}
				componentDict[arrayComponent.GetType()] = arrayComponent;
			}
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
        if (!TryGetComponent(out UnlockRequirementsComponent unlockRequirementsComponent)
            || unlockRequirementsComponent.requirements == null)
            return true;
        
        foreach (var unlockRequirement in unlockRequirementsComponent.requirements)
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