using Godot;

[GlobalClass][Tool]
public partial class SufficientFoodRequirement : Requirement
{
    public override bool IsSatisfied()
    {
        var currencyChangeAnalyser = InjectionManager.Get<MapCurrencyChangeAnalyser>();
        var overallMapTurnDelta = currencyChangeAnalyser.GetOverallMapTurnDelta();
        if (!overallMapTurnDelta.TryGetValue(CurrencyType.Food, out int foodDelta))
            return false;

        var inventoryManager = InjectionManager.Get<InventoryManager>();
        var currentFoodCount = inventoryManager.GetCurrentCurrencyQuantity(CurrencyType.Food);

        var residentManager = InjectionManager.Get<ResidentManager>();
        var residentTotalConsumption = residentManager.GetTotalResidentFoodConsumption();
        if (!residentTotalConsumption.TryGetValue(CurrencyType.Food, out var residentFoodConsumption))
            residentFoodConsumption = 0;
        
        return currentFoodCount + foodDelta + residentFoodConsumption >= 0;
    }
}
