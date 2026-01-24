using Godot;

public partial class UnlockRequirementUI : Control
{
    [Export]
    private TextureRect icon;

    [Export]
    private Label text;

    [Export]
    private Control completeIndicator;

    public void Setup(Requirement requirement)
    {
        var requirementUiMappingList = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        icon.Texture = requirementUiMappingList.GetIconForRequirement(requirement);
        text.Text = requirementUiMappingList.GetTextForUnlockRequirement(requirement);
        bool isSatisfied = requirement.IsSatisfied();
        completeIndicator.Visible = isSatisfied;
        icon.SelfModulate = new Color(icon.SelfModulate.R, icon.SelfModulate.G, icon.SelfModulate.B, isSatisfied ? 0.5f : 1f);
        text.SelfModulate = new Color(text.SelfModulate.R, text.SelfModulate.G, text.SelfModulate.B, isSatisfied ? 0.5f : 1f);
    }
}
