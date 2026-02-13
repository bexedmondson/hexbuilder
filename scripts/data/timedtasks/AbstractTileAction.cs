using Godot;

[GlobalClass]
public abstract partial class AbstractTileAction : Resource
{
    public abstract void DoAction(Vector2I cell);
}
