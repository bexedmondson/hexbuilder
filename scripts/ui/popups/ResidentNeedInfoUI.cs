using System.Collections.Generic;
using Godot;

public partial class ResidentNeedInfoUI : Control
{
    [Export]
    private Label text;

    [Export]
    private TextureRect needIcon;

    [Export]
    private TextureRect intensityIcon;

    public void SetText(string textToSet)
    {
        text.Text = textToSet;
    }

    public void SetNeedIcon(Texture2D texture)
    {
        if (texture != null)
            needIcon.Texture = texture;
    }

    public void SetIntensity(int happinessEffect)
    {
        var happinessIconMapping = InjectionManager.Get<IconMapper>().happinessMap;
        if (happinessIconMapping.TryGetValue(happinessEffect, out var happinessTexture))
        {
            intensityIcon.Texture = happinessTexture;
            return;
        }

        int closestHappiness = int.MinValue;
        foreach (var kvp in happinessIconMapping)
        {
            if (closestHappiness == int.MinValue)
            {
                closestHappiness = kvp.Key;
                continue;
            }

            int prevBest = Mathf.Abs(happinessEffect - closestHappiness);
            if (Mathf.Abs(happinessEffect - kvp.Key) < prevBest)
                closestHappiness = kvp.Key;
        }

        if (closestHappiness != int.MinValue)
            intensityIcon.Texture = happinessIconMapping[closestHappiness];
    }
}
