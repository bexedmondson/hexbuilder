using Godot;

public partial class StorageCapacityIndicator : TextureRect
{
    [Export]
    private Texture2D fullIcon;
    
    [Export]
    private Texture2D overflowingIcon;

    public void SetNotFull()
    {
        this.SelfModulate = Colors.Transparent;
        this.Texture = null;
    }
    
    public void SetFull()
    {
        this.SelfModulate = Colors.White;
        this.Texture = fullIcon;
    }
    
    public void SetOverflowing()
    {
        this.SelfModulate = Colors.White;
        this.Texture = overflowingIcon;
    }
}
