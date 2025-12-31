using Godot;

public abstract partial class Popup : Control
{
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("ui_cancel"))
            Close();
        else if (Input.IsActionJustReleased("ui_accept"))
            Confirm();
    }

    public virtual void Close()
    {
        this.SetVisible(false);
    }

    public virtual void Confirm() {}
}
