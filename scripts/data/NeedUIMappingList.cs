using Godot;

[GlobalClass][Tool]
public partial class NeedUIMappingList : Resource
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
    private string allTimeBuildText;
    
    [ExportGroup("Food")]
    [Export]
    private Texture2D foodIcon;
    [Export]
    private string sufficientFoodText;
    

    public Texture2D GetIconForNeedSatisfactionRequirement(DataRequirement requirement)
    {
        switch (requirement)
        {
            case ResidentBetterHousingRequirement _:
                return houseIcon;
            case IsHousedRequirement _:
                return houseIcon;
        }

        return null; //TODO default icon?
    }
    
    public Texture2D GetIconForNeedSatisfactionRequirement(Requirement requirement)
    {
        switch (requirement)
        {
            case PopulationRequirement _:
                return populationIcon;
            case AllTimeTileBuildRequirement _:
                return allTimeBuildIcon;
            case SufficientFoodRequirement _:
                return foodIcon;
        }

        return null;
    }
    
    public string GetTextForNeedSatisfactionRequirement(DataRequirement requirement)
    {
        switch (requirement)
        {
            case ResidentBetterHousingRequirement _:
                return betterHousingText;
            case IsHousedRequirement _:
                return isHousedText;
        }

        return string.Empty;
    }
    
    public string GetTextForNeedSatisfactionRequirement(Requirement requirement)
    {
        switch (requirement)
        {
            case PopulationRequirement _:
                return populationText;
            case AllTimeTileBuildRequirement allTimeTileBuildRequirement:
                return allTimeBuildText.Replace("{building}", allTimeTileBuildRequirement.requiredTileBuildCount.tile.GetFileName());
            case SufficientFoodRequirement _:
                return sufficientFoodText;
        }

        return string.Empty;
    }
}
