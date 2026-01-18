using Godot;
using Godot.Collections;

public partial class ToastManager : CanvasLayer, IInjectable
{
	[Export]
	public PackedScene LabelResource;

	private Dictionary<ToastDirection, Array<ToastLabel>> directionArrayMap = new(){
		{ ToastDirection.TopLeft, new() },
		{ ToastDirection.TopCenter, new() },
		{ ToastDirection.TopRight, new() },
		{ ToastDirection.BottomLeft, new() },
		{ ToastDirection.BottomCenter, new() },
		{ ToastDirection.BottomRight, new() }
	};

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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Layer = 128;

		// TODO: We need Debounce function
		// Connect signal resize to _on_resize

	}
	// get_tree().get_root().connect("size_changed", _on_resize, 1)
	
	private void AddNewLabel(ToastConfig config)
	{
		// Create a new label
		var label = LabelResource.Instantiate<ToastLabel>();
		this.AddChild(label);
		label.RemoveLabelAction = RemoveLabelFromArray;

		directionArrayMap[config.direction].Insert(0, label);

		// Configuration of the label
		label.Init(config);

		// Move all labels to new positions when a new label is added
		MovePositions(config.direction);
	}

	public void MovePositions(ToastDirection direction)
	{
		for (int i = 0; i < directionArrayMap[direction].Count; i++)
		{
			var label = directionArrayMap[direction][i];
			label.MoveTo(i);
		}
	}

	public void RemoveLabelFromArray(ToastLabel label)
	{
		directionArrayMap[label.Direction].Remove(label);
	}

	//# Event resize
	protected void _OnResize()
	{
		foreach(var kvp in directionArrayMap)
		{
			foreach (var label in kvp.Value)
			{
				label.UpdateXPosition();
			}
		}
	}

	public void Show(ToastConfig config)
	{
		AddNewLabel(config);
	}
}