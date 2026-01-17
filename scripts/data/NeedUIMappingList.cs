using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;

[GlobalClass][Tool]
public partial class NeedUIMappingList : Resource
{
    [ExportGroup("Resident Better Housing")]
    [Export]
    private Texture2D betterHousingIcon;
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

    public Texture2D GetIconForNeedSatisfactionRequirement(DataRequirement requirement)
    {
        switch (requirement)
        {
            case ResidentBetterHousingRequirement _:
                return betterHousingIcon;
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
        }

        return null;
    }
    
    public string GetTextForNeedSatisfactionRequirement(DataRequirement requirement)
    {
        switch (requirement)
        {
            case ResidentBetterHousingRequirement _:
                return betterHousingText;
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
        }

        return string.Empty;
    }
}
