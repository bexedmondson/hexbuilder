using System.Collections.Generic;
using Godot;

public partial class UnlockedCellAdjacencyUI : Control
{
    [Export]
    private Control effectGivenUI;
    
    [Export]
    private Control effectReceivedUI;

    [Export]
    private Label name;

    [Export]
    private Label direction;

    [Export]
    private CurrencyDisplay giveEffectDisplay;
    
    [Export]
    private CurrencyDisplay receiveEffectDisplay;

    private readonly Dictionary<Vector2I, string> coordsToDirectionMap = new(){
        { Vector2I.Down, "NW" },
        { Vector2I.Left, "NE"},
        { Vector2I.Up, "SE"},
        { Vector2I.Right, "SW"},
        { new Vector2I(1, -1), "S"},
        { new Vector2I(-1, 1), "N"}
    };

    public void Setup(CustomTileData adjacentTileData, Vector2I centreCell, Vector2I adjacentCell)
    {
        name.Text = adjacentTileData.GetFileName();
        direction.Text = coordsToDirectionMap[centreCell - adjacentCell];

        effectGivenUI.Visible = false;
        effectReceivedUI.Visible = false;
    }

    public void SetGivenEffects(CurrencySum givenEffects)
    {
        effectGivenUI.Visible = givenEffects.Count > 0;
        giveEffectDisplay.DisplayCurrencyAmount(givenEffects);
    }
    
    public void SetReceivedEffects(CurrencySum receivedEffects)
    {
        effectReceivedUI.Visible = receivedEffects.Count > 0;
        receiveEffectDisplay.DisplayCurrencyAmount(receivedEffects);
    }
}
