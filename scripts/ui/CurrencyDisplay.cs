using System.Collections.Generic;
using Godot;

public partial class CurrencyDisplay : Control
{
    [Export]
    private Godot.Collections.Dictionary<CurrencyType, Resource> currencyIcons;

    [Export]
    private bool showTurnChange = false;

    [Export]
    private PackedScene singleCurrencyDisplayScene;

    private MapController mapController;

    private Dictionary<CurrencyType, SingleCurrencyDisplay> currencyDisplays = new();

    public void DisplayCurrencyAmount(CurrencySum currencySum)
    {
        CurrencySum turnChange = new();
        if (showTurnChange)
        {
            mapController ??= InjectionManager.Get<MapController>();
            foreach (var cell in mapController.BaseMapLayer.GetUsedCells())
            {
                if (mapController.GetCellStatus(cell) != CellStatus.Unlocked)
                    continue;

                var cellTileData = mapController.BaseMapLayer.GetCellCustomData(cell);
                if (cellTileData?.baseTurnCurrencyChange != null)
                    turnChange.Add(new CurrencySum(cellTileData.baseTurnCurrencyChange));
            
                turnChange.Add(mapController.CalculateAdjacencyEffects(cell, cellTileData));
            }
        }
        
        foreach (var kvp in currencySum)
        {
            var currencyType = kvp.Key;
            var amount = kvp.Value;
            
            var hasDisplay = currencyDisplays.TryGetValue(currencyType, out var existingSingleCurrencyDisplay);

            if (!hasDisplay)
            {
                existingSingleCurrencyDisplay = singleCurrencyDisplayScene.Instantiate<SingleCurrencyDisplay>();
                currencyDisplays[currencyType] = existingSingleCurrencyDisplay;
                this.AddChild(existingSingleCurrencyDisplay);
            }

            if (showTurnChange)
            {
                int delta = 0;
                turnChange.TryGetValue(kvp.Key, out delta);
                existingSingleCurrencyDisplay.Set(currencyIcons[currencyType] as Texture2D, amount, delta);
                existingSingleCurrencyDisplay.ShowDelta(true);
            }
            else
            {
                existingSingleCurrencyDisplay.Set(currencyIcons[currencyType] as Texture2D, amount);
                existingSingleCurrencyDisplay.ShowDelta(false);
            }
            
            /*var textureRect = new TextureRect();
            textureRect.Texture = currencyIcons[currencyType] as Texture2D;
            textureRect.CustomMinimumSize = Vector2.One * 40;
            textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional;
            this.AddChild(textureRect);

            var label = new Label();
            label.LabelSettings = new LabelSettings() { FontSize = 25 };
            label.Text = amount.ToString() + "   ";
            this.AddChild(label);

            if (showTurnChange)
            {
                var changeLabel = new Label();
                changeLabel.LabelSettings = new LabelSettings() { FontSize = 25 };
                
                int delta = 0;
                turnChange.TryGetValue(kvp.Key, out delta);
                string prefix = delta < 0 ? "-" : "+";
                changeLabel.Text = $"({prefix}{delta})";
                this.AddChild(changeLabel);
            }*/
        }
    }

    public void Cleanup()
    {
        for (int i = this.GetChildCount() - 1; i >= 0; i--)
        {
            this.GetChild(i).QueueFree();
        }
        
        currencyDisplays.Clear();
    }
}