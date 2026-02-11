using Godot;

public partial class CloudParticles : CpuParticles2D
{
    public override void _EnterTree()
    {
        base._EnterTree();
        this.Emitting = true; //purely because i want it turned off in editor
    }
}
