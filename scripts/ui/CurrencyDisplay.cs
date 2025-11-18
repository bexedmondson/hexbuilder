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