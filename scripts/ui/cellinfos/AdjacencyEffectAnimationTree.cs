using Godot;

public partial class AdjacencyEffectAnimationTree : AnimationTree
{
    [Export]
    private Sprite2D icon;
    
    public void SetIsDrawback(bool isDrawback)
    {
        this.Set("parameters/isDrawback/blend_amount", isDrawback ? 1.0 : 0.0);
    }

    public void SetCurrencyType(CurrencyType type)
    {
        if (icon != null)
            icon.Texture = InjectionManager.Get<InventoryManager>().GetIcon(type);
    }
}
