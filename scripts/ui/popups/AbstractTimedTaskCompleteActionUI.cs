using Godot;

public abstract partial class AbstractTimedTaskCompleteActionUI : Control
{
    public abstract void Setup(AbstractTileAction completeAction, Vector2I cell);
}
