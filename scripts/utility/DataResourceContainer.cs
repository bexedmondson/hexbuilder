using Godot;

public partial class DataResourceContainer : Node, IInjectable
{
    [Export]
    public NeedConfigList needConfigList;

    [Export]
    public RequirementUIMappingList requirementUIMappingList;
    
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
