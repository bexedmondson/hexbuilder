using Godot;

[GlobalClass][Tool]
public partial class PopulationRequirement : Requirement
{
    [Export]
    public int threshold;

    [Export]
    private Comparison comparison;

    public override bool IsSatisfied()
    {
        var residentCount = InjectionManager.Get<ResidentManager>().AllResidents.Length;
        return residentCount.IsPass(comparison, threshold);
    }
}