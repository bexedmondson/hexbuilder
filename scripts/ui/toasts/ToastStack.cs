using System.Collections.Generic;
using Godot;

public partial class ToastStack : VBoxContainer //well isn't this a cute class name
{
	[Export]
	private string stackId;
	
	[Export]
	public PackedScene toastLabelScene;

	private ToastManager toastManager;
	private List<ToastLabel> toasts = new();

	public override void _EnterTree()
	{
		base._EnterTree();
		toastManager = InjectionManager.Get<ToastManager>();
		toastManager.RegisterStack(this, stackId);
	}

	public void MakeToast(ToastConfig config)
	{
		// Create a new label
		var toast = toastLabelScene.Instantiate<ToastLabel>();
		this.AddChild(toast);
		toasts.Add(toast);
		toast.RemoveLabelAction = RemoveLabelFromArray;

		// Configuration of the label
		toast.Init(config);
	}

	private void RemoveLabelFromArray(ToastLabel toast)
	{
		toasts.Remove(toast);
	}

    public override void _ExitTree()
    {
        base._ExitTree();
        toastManager.DeregisterStack(this, stackId);
    }
}
