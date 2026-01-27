#if TOOLS
using Godot;

[Tool]
public partial class TilePlacementConfigurator : EditorPlugin
{
	private PackedScene mainPanelScene = ResourceLoader.Load<PackedScene>("res://addons/tile_placement_configurator/tile_placement_configurator.tscn");
	private TilePluginParentNode mainPanelInstance;

	private RedoSplitEditorInspectorPlugin redoSplitEditorInspectorPlugin;
	
	public override void _EnterTree()
	{
		mainPanelInstance = (TilePluginParentNode)mainPanelScene.Instantiate();
		EditorInterface.Singleton.GetEditorMainScreen().AddChild(mainPanelInstance);
		_MakeVisible(false);

		redoSplitEditorInspectorPlugin = new();
		//AddInspectorPlugin(redoSplitEditorInspectorPlugin);
	}

	public override void _Ready() 
	{
		base._Ready();
		MainScreenChanged += _ => mainPanelInstance.AddTileData();
	}

	public override void _ExitTree()
	{
		//RemoveInspectorPlugin(redoSplitEditorInspectorPlugin);
		if (mainPanelInstance != null)
			mainPanelInstance.QueueFree();
	}
	
	public override bool _HasMainScreen()
	{
		return true;
	}

	public override void _MakeVisible(bool visible)
	{
		if (mainPanelInstance != null)
			mainPanelInstance.Visible = visible;
	}

	public override string _GetPluginName()
	{
		return "Tiles";
	}

	public override Texture2D _GetPluginIcon()
	{
		return GD.Load<Texture2D>("res://addons/tile_placement_configurator/icon.svg");
	}
}
#endif