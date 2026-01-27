
using System.Collections.Generic;
using Godot;

public partial class ResidentInfoUI : Control
{
    [Export]
    private Label residentLabel;

    [Export]
    private TextureRect happinessIcon;

    [Export]
    private Control noHouseLabel;

    [Export]
    private Label employmentLabel;

    [Export]
    private Control needInfoParent;

    [Export]
    private PackedScene needInfoScene;

    private List<ResidentNeedInfoUI> needInfoUIs = new();
    
    public void SetResident(ResidentState residentState)
    {
        residentLabel.Text = residentState.Name;
        happinessIcon.Texture = InjectionManager.Get<IconMapper>().happinessMap[residentState.happiness];
        noHouseLabel.Visible = !residentState.HasHouse;
        employmentLabel.Text = residentState.IsBusy ? $"is employed at {residentState.GetWorkplaceOrJobName()}" : "is not employed";

        var needUIMapping = InjectionManager.Get<DataResourceContainer>().requirementUIMappingList;
        
        foreach (var activeNeed in residentState.activeNeeds)
        {
            if (activeNeed.IsSatisfied(residentState))
                continue;
            
            var needInfoUI = needInfoScene.Instantiate<ResidentNeedInfoUI>();

            foreach (var satisfactionRequirement in activeNeed.satisfactionRequirements)
            {
                //need to figure out handling >1 requirement and regular requirements
                if (satisfactionRequirement is DataRequirement dataRequirement)
                {
                    needInfoUI.SetText(needUIMapping.GetTextForNeedSatisfactionRequirement(dataRequirement));
                    needInfoUI.SetNeedIcon(needUIMapping.GetIconForRequirement(dataRequirement));
                }
                else if (satisfactionRequirement is Requirement requirement)
                {
                    needInfoUI.SetText(needUIMapping.GetTextForNeedSatisfactionRequirement(requirement));
                    needInfoUI.SetNeedIcon(needUIMapping.GetIconForRequirement(requirement));
                }
                
                needInfoUI.SetIntensity(activeNeed.unsatisfiedHappinessPenalty);
            }
            
            needInfoParent.AddChild(needInfoUI);
            needInfoUIs.Add(needInfoUI);
        }
    }

    public void OnToggleNeedBubbles(bool toggleState)
    {
        needInfoParent.Visible = toggleState;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        foreach (var needInfoUI in needInfoUIs)
        {
            needInfoUI.QueueFree();
        }
    }
}
