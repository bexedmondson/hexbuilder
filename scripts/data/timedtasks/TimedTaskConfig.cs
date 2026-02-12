using System;
using Godot;

[GlobalClass]
public partial class TimedTaskConfig : Resource
{
    [Export]
    public string jobName { get; protected set; }

    [Export]
    public Texture2D icon { get; protected set; }

    [Export(PropertyHint.Range, "0,100,")]
    public int turnDuration { get; protected set; }

    [Export]
    public Godot.Collections.Array<AbstractTileAction> completeActions { get; protected set; } = new();

    [Export]
    public Godot.Collections.Array<AbstractTimedTaskComponent> components { get; protected set; } = new();
    
    protected System.Collections.Generic.Dictionary<Type, AbstractTimedTaskComponent> componentDict = null;
    
    public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : AbstractTimedTaskComponent
    {
        if (componentDict == null)
        {
            componentDict = new System.Collections.Generic.Dictionary<Type, AbstractTimedTaskComponent>();
			
            foreach (var arrayComponent in components)
            {
                if (arrayComponent == null)
                {
                    GD.PrintErr($"Null timed task component found in {this.jobName}!");
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

    public bool CanStart<T>(T data) where T : new()
    {
        //if no component, no requirements to start so it's fine
        if (!TryGetComponent(out StartRequirementsTimedTaskComponent startRequirements))
            return true;

        //if we have a component but nothing in it, there is no way to start the task so it's not fine
        return startRequirements.requirements != null && RequirementCalculation.GetAreRequirementsSatisfied(startRequirements.requirements, data);
    }

    public bool ShouldCancel<T>(T data) where T : new()
    {
        //if no component, no requirements to cancel so it's fine
        if (!TryGetComponent(out CancelRequirementsTimedTaskComponent cancelRequirements))
            return true;
        return cancelRequirements.requirements != null && RequirementCalculation.GetAreRequirementsSatisfied(cancelRequirements.requirements, data);
    }
}
