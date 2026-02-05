using Godot;

public partial class UnlockRequirementUI : Control
{
    [Export]
    private TextureRect icon;

    [Export]
    private Label text;

    [Export]
    private Control completeIndicator;

    public void Setup<T>(AbstractRequirement requirement, T data) where T : new()
    {
        var requirementUiMappingList = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        bool isSatisfied = false;
        if (requirement is DataRequirement dataRequirement)
            SetupDataRequirement(dataRequirement, data, out isSatisfied);
        else if (requirement is Requirement standardRequirement)
            SetupRequirement(standardRequirement, out isSatisfied);
        
        completeIndicator.Visible = isSatisfied;
        icon.SelfModulate = new Color(icon.SelfModulate.R, icon.SelfModulate.G, icon.SelfModulate.B, isSatisfied ? 0.5f : 1f);
        text.SelfModulate = new Color(text.SelfModulate.R, text.SelfModulate.G, text.SelfModulate.B, isSatisfied ? 0.5f : 1f);
    }

    private void SetupDataRequirement<T>(DataRequirement dataRequirement, T data, out bool isSatisfied) where T : new()
    {
        var requirementUiMappingList = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        icon.Texture = requirementUiMappingList.GetIconForRequirement(dataRequirement, RequirementUIType.Unlock);
        text.Text = requirementUiMappingList.GetTextForRequirement(dataRequirement, RequirementUIType.Unlock);
        isSatisfied = RequirementCalculation.GetDataRequirementSatisfaction(dataRequirement.GetDataRequirementProcessor(), data);
    }

    private void SetupRequirement(Requirement requirement, out bool isSatisfied)
    {
        var requirementUiMappingList = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        icon.Texture = requirementUiMappingList.GetIconForRequirement(requirement, RequirementUIType.Unlock);
        text.Text = requirementUiMappingList.GetTextForRequirement(requirement, RequirementUIType.Unlock);
        isSatisfied = requirement.IsSatisfied();
    }
}
