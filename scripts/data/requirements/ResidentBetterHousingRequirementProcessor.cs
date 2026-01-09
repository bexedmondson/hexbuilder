public class ResidentBetterHousingRequirementProcessor(ResidentBetterHousingRequirement dataRequirement) : ResidentStateRequirementProcessor<ResidentBetterHousingRequirement>(dataRequirement)
{
    public override bool IsSatisfied(ResidentState resident)
    {
        var housingManager = InjectionManager.Get<HousingManager>();
        if (!housingManager.TryGetHouseForResident(resident, out var residence))
            return false;
        
        return !dataRequirement.unacceptableHousing.Contains(residence.tileData);
    }
}
