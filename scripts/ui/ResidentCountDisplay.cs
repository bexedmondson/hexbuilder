using Godot;

public partial class ResidentCountDisplay : Label
{
    public override void _EnterTree()
    {
        base._EnterTree();
        var eventDispatcher = InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Add<ResidentCountUpdatedEvent>(OnCountUpdated);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        var eventDispatcher = InjectionManager.Get<EventDispatcher>();
        eventDispatcher.Remove<ResidentCountUpdatedEvent>(OnCountUpdated);
    }

    private void OnCountUpdated(ResidentCountUpdatedEvent e)
    {
        this.Text = e.count.ToString();
    }
}
