using Godot;

public partial class UnlockRequirementUI : Control
{
    [Export]
    private TextureRect icon;

    [Export]
    private Label text;

    [Export]
    private Control completeIndicator;

    private static Color semitransparentWhite = new Color(1, 1, 1, 0.5f);

    public void Setup(Requirement requirement)
    {
        var requirementUiMappingList = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        icon.Texture = requirementUiMappingList.GetIconForRequirement(requirement);
        text.Text = requirementUiMappingList.GetTextForUnlockRequirement(requirement);
        bool isSatisfied = requirement.IsSatisfied();
        completeIndicator.Visible = isSatisfied;
        icon.SelfModulate = isSatisfied ? semitransparentWhite : Colors.White;
        text.SelfModulate = isSatisfied ? semitransparentWhite : Colors.White;
    }
}
