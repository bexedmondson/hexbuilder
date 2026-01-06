using Godot;
using Godot.Collections;

public partial class IconMapper : Control, IInjectable
{
    [Export]
    private Dictionary<int, AtlasTexture> happinessIconMap = new();
    public Dictionary<int, AtlasTexture> happinessMap => happinessIconMap;
    
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
}
