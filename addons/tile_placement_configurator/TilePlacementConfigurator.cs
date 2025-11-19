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
		HandleTileDB(true);
		
		mainPanelInstance = (Control)mainPanelScene.Instantiate();
		EditorInterface.Singleton.GetEditorMainScreen().AddChild(mainPanelInstance);
		_MakeVisible(false);
	}

	public override void _EnablePlugin()
	{
		base._EnablePlugin();
		HandleTileDB(true);
	}
	
	public override void _DisablePlugin()
	{
		base._DisablePlugin();
		HandleTileDB(false);
	}

	public override void _ExitTree()
	{
		HandleTileDB(false);
		if (mainPanelInstance != null)
			mainPanelInstance.QueueFree();
	}

	private void HandleTileDB(bool shouldDbExist)
	{
		if (shouldDbExist && tileDatabase == null)
		{
			tileDatabase = new TileDatabase();
		}
		else if (tileDatabase != null)
		{
			if (InjectionManager.Has<TileDatabase>())
				InjectionManager.Deregister(tileDatabase);
			tileDatabase = null;
		}
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