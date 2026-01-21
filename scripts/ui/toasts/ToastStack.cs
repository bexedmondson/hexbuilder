using System.Collections.Generic;
using Godot;

[Tool]
public partial class ToastStack : VBoxContainer //well isn't this a cute class name
{
	[Export]
	private string stackId;
	
	[Export]
	public PackedScene toastLabelScene;

	[ExportToolButton("label")]
	private Callable labelCall => Callable.From(TestLabel);

	private ToastManager toastManager;
	private List<ToastLabel> toasts = new();

	public override void _EnterTree()
	{
		base._EnterTree();
		toastManager = InjectionManager.Get<ToastManager>();
		if (toastManager != null)
			toastManager.RegisterStack(this, stackId);
	}

	private void TestLabel()
	{
		MakeToast("testing 1 2 3 4 5");
	}

	public void MakeToast(string toastText)
	{
		// Create a new label
		var toast = toastLabelScene.Instantiate<ToastLabel>();
		this.AddChild(toast);
		toasts.Add(toast);
		toast.RemoveLabelAction = RemoveLabelFromArray;

		// Configuration of the label
		toast.Init(toastText);
	}

	private void RemoveLabelFromArray(ToastLabel toast)
	{
		toasts.Remove(toast);
	}

    public override void _ExitTree()
    {
        base._ExitTree();
        if (toastManager != null)
	        toastManager.DeregisterStack(this, stackId);
    }
}
