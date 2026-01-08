using Godot;

[GlobalClass][Tool]
public partial class RandomnessRequirement : Requirement
{
    [Export(PropertyHint.Range, "0,1")]
    private float threshold;

    public override bool IsSatisfied()
    {
        return GD.Randf() >= threshold;
    }
}