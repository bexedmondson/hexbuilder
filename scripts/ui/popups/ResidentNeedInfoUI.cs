using Godot;

public partial class ResidentNeedInfoUI : Control
{
    [Export]
    private Label text;

    [Export]
    private TextureRect icon;

    public void SetText(string textToSet)
    {
        text.Text = textToSet;
    }

    public void SetIcon(Texture2D texture)
    {
        if (texture != null)
            icon.Texture = texture;
    }
}
