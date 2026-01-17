using Godot;

[GlobalClass][Tool]
public partial class SufficientFoodRequirement : Requirement
{
    public override bool IsSatisfied()
    {
        var residentCount = InjectionManager.Get<ResidentManager>().AllResidents.Length;
        var inventoryManager = InjectionManager.Get<InventoryManager>();
        return inventoryManager.CanAfford(new CurrencySum(CurrencyType.Food, residentCount));
    }
}
