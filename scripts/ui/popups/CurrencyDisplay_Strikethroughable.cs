using Godot;

public partial class CurrencyDisplay_Strikethroughable : CurrencyDisplay
{
    [Export]
    private Control strikethrough;

    [Export]
    private Color defaultColor = Colors.White;

    [Export]
    private Color struckOutColor = Color.FromHtml("ffffffb3");

    public override void _Ready()
    {
        base._Ready();
        this.Modulate = defaultColor;
    }

    public void SetStrikethrough(bool strikeActive)
    {
        this.Modulate = strikeActive ? struckOutColor : defaultColor;
        strikethrough.Visible = strikeActive;
    }
}
