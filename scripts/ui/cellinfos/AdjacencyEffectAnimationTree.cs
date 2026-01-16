using Godot;

public partial class AdjacencyEffectAnimationTree : AnimationTree
{
    public void SetIsDrawback(bool isDrawback)
    {
        this.Set("parameters/isDrawback/blend_amount", isDrawback ? 1.0 : 0.0);
    }

    public void SetCurrencyType(CurrencyType type)
    {
        this.Set("parameters/StateMachine/conditions/isFabric", type == CurrencyType.Fabric);
        this.Set("parameters/StateMachine/conditions/isFood", type == CurrencyType.Food);
        this.Set("parameters/StateMachine/conditions/isWood", type == CurrencyType.Wood);
        this.Set("parameters/StateMachine/conditions/isStone", type == CurrencyType.Stone);
    }
}
