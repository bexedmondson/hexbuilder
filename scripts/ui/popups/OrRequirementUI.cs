using Godot;

public partial class OrRequirementUI : RequirementUI
{
    [Export]
    private Control subRequirementParent;

    public override void Setup<T>(AbstractRequirement requirement, T data)
    {
        for (int i = subRequirementParent.GetChildCount() - 1; i >= 0; i--)
        {
            subRequirementParent.GetChild(i).QueueFree();
        }
        
        if (requirement is not OrRequirement orRequirement)
        {
            GD.PushError("Trying to use OrRequirementUI with non-OrRequirement!");
            return;
        }

        var factory = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        foreach (var subRequirement in orRequirement.requirements)
        {
            var newRequirementUI = factory.CreateUIInstanceForRequirement(subRequirement);
            newRequirementUI.Setup(subRequirement, data);
            subRequirementParent.AddChild(newRequirementUI);
        }
    }
}
