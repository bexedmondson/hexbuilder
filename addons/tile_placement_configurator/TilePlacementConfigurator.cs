#if TOOLS
using Godot;

[Tool]
public partial class TilePlacementConfigurator : EditorPlugin
{
	private PackedScene mainPanelScene = ResourceLoader.Load<PackedScene>("res://addons/tile_placement_configurator/tile_placement_configurator.tscn");
	private Control mainPanelInstance;

	private TileDatabase tileDatabase;
	
	public override void _EnterTree()
	{
		tileDatabase = new TileDatabase();
		
		mainPanelInstance = (Control)mainPanelScene.Instantiate();
		EditorInterface.Singleton.GetEditorMainScreen().AddChild(mainPanelInstance);
		_MakeVisible(false);
	}

	public override void _ExitTree()
	{
		if (mainPanelInstance != null)
			mainPanelInstance.QueueFree();
		
		if (InjectionManager.Has<TileDatabase>())
			InjectionManager.Deregister(tileDatabase);
	}

	public override bool _HasMainScreen()
	{
		return true;
	}

	public override void _MakeVisible(bool visible)
	{
		if (mainPanelInstance != null)
		{
			mainPanelInstance.Visible = visible;
		}
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