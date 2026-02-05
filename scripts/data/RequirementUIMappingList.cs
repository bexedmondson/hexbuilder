using Godot;

[GlobalClass][Tool]
public partial class RequirementUIMappingList : Resource
{
    [ExportGroup("Housing")]
    [Export]
    private Texture2D houseIcon;
    [Export]
    private string isHousedText;
    [Export]
    private string betterHousingText;
    
    [ExportGroup("Population")]
    [Export]
    private Texture2D populationIcon;
    [Export]
    private string populationText;
    
    [ExportGroup("Build Requirements")]
    [Export]
    private Texture2D allTimeBuildIcon;
    [Export]
    private string allTimeBuildNeedText;
    [Export]
    private string allTimeBuildUnlockText;
    [Export]
    private string currentTileBuildUnlockText;

    [ExportGroup("Currency")]
    [Export]
    private string lifetimeGainedText;
    
    [ExportGroup("Food")]
    [Export]
    private Texture2D foodIcon;
    [Export]
    private string sufficientFoodText;

    [ExportGroup("Housemates")]
    [Export]
    private Texture2D noDepressedHousematesIcon;
    [Export]
    private string noDepressedHousematesText;
    
    public Texture2D GetIconForRequirement(AbstractRequirement abstractRequirement, RequirementUIType requirementUIType)
    {
        switch (abstractRequirement)
        {
            case ResidentBetterHousingRequirement _:
                return houseIcon;
            case IsHousedRequirement _:
                return houseIcon;
            case ResidentNoDepressedHousematesRequirement _:
                return noDepressedHousematesIcon;
            case PopulationRequirement _:
                return populationIcon;
            case AllTimeTileBuildRequirement _:
                return allTimeBuildIcon;
            case CurrentTilePresenceCountRequirement _:
                return allTimeBuildIcon;
            case SufficientFoodRequirement _:
                return foodIcon;
            case LifetimeGainedRequirement lifetimeGainedRequirement:
                return InjectionManager.Get<InventoryManager>().GetIcon(lifetimeGainedRequirement.currency);
        }

        return null; //TODO default icon?
    }
    
    public string GetTextForRequirement(AbstractRequirement abstractRequirement, RequirementUIType requirementUIType)
    {
        switch (abstractRequirement)
        {
            case ResidentBetterHousingRequirement _:
                return betterHousingText;
            case IsHousedRequirement _:
                return isHousedText;
            case ResidentNoDepressedHousematesRequirement _:
                return noDepressedHousematesText;
            case PopulationRequirement _:
                return populationText;
            case AllTimeTileBuildRequirement allTimeTileBuildRequirement when requirementUIType == RequirementUIType.Need:
                return allTimeBuildNeedText.Replace("{building}", allTimeTileBuildRequirement.requiredTileBuildCount.tile.GetFileName());
            case AllTimeTileBuildRequirement allTimeTileBuildRequirement: //when requirementUIType != RequirementUIType.Need
                return allTimeBuildUnlockText.Replace("{building}", allTimeTileBuildRequirement.requiredTileBuildCount.tile.GetFileName());
            case SufficientFoodRequirement _:
                return sufficientFoodText;
            case CurrentTilePresenceCountRequirement currentTilePresenceCountRequirement:
                return currentTileBuildUnlockText.Replace("{building}", currentTilePresenceCountRequirement.requiredTileBuildCount.tile.GetFileName());
            case LifetimeGainedRequirement ltgr:
                return lifetimeGainedText.Replace("{currency}", ltgr.currency.ToString().ToLower());
        }

        return string.Empty;
    }
}
