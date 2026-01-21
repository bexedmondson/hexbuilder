using Godot;

public partial class ToastManager : Node, IInjectable
{
	private System.Collections.Generic.Dictionary<string, ToastStack> stackById = new();

	public override void _EnterTree()
	{
		base._EnterTree();
		InjectionManager.Register(this);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		InjectionManager.Deregister(this);
	}

	public void RegisterStack(ToastStack stack, string stackId)
	{
		stackById[stackId] = stack;
	}
	
	public void DeregisterStack(ToastStack stack, string stackId)
	{
		stackById.Remove(stackId);
	}

	public void RequestToast(ToastConfig config, string stackId)
	{
		if (!stackById.TryGetValue(stackId, out var stack))
		{
			GD.PushWarning($"[ToastManager] Toast stack for ID {stackId} not found. Discarding toast {config.text}");
			return;
		}
		
		stack.MakeToast(config);
	}
}