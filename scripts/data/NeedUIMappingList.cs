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

    public Texture2D GetIconForNeedSatisfactionRequirement(DataRequirement requirement)
    {
        switch (requirement)
        {
            case ResidentBetterHousingRequirement _:
                return betterHousingIcon;
        }

        return null; //TODO default icon?
    }
    
    public string GetTextForNeedSatisfactionRequirement(DataRequirement requirement)
    {
        switch (requirement)
        {
            case ResidentBetterHousingRequirement betterHousingRequirement:
                return GetBetterHousingText(betterHousingRequirement);
        }

        return string.Empty;
    }

    private string GetBetterHousingText(ResidentBetterHousingRequirement betterHousingRequirement)
    {
        return betterHousingText;
    }
}
