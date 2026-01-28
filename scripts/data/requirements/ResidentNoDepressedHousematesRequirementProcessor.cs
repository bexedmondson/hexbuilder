
public class ResidentNoDepressedHousematesRequirementProcessor(ResidentNoDepressedHousematesRequirement dataRequirement) : ResidentStateRequirementProcessor<ResidentNoDepressedHousematesRequirement>(dataRequirement)
{
    public override bool IsSatisfied(ResidentState resident)
    {
        var housingManager = InjectionManager.Get<HousingManager>();
        if (!housingManager.TryGetHouseForResident(resident, out var residence))
            return false;

        foreach (var housemate in residence.occupants)
        {
            if (resident == housemate)
                continue;

            if (housemate.happiness < dataRequirement.minimumAcceptableHappiness)
                return false;
        }

        return true;
    }
}
