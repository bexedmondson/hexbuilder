using Godot;

public partial class StorageCapacityIndicator : TextureRect
{
    [Export]
    private Texture2D fullIcon;
    
    [Export]
    private Texture2D overflowingIcon;

    private static Color transparent = new Color(0, 0, 0, 0);
    private static Color white = new Color(1, 1, 1);

    public void SetNotFull()
    {
        this.SelfModulate = transparent;
        this.Texture = null;
    }
    
    public void SetFull()
    {
        this.SelfModulate = white;
        this.Texture = fullIcon;
    }
    
    public void SetOverflowing()
    {
        this.SelfModulate = white;
        this.Texture = overflowingIcon;
    }
}
