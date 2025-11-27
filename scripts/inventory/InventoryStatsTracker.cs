
using System.Collections.Generic;

public class InventoryStatsTracker : IInjectable
{
    public InventoryStatsTracker()
    {
        InjectionManager.Register(this);
    }

    ~InventoryStatsTracker()
    {
        InjectionManager.Deregister(this);
    }

    private CurrencySum gainedStats = new();
    private CurrencySum usedStats = new();

    public void OnCurrencyChange(CurrencySum currencyDelta)
    {
        foreach (var kvp in currencyDelta)
        {
            if (kvp.Value > 0)
                gainedStats.Add(kvp.Key, kvp.Value);
            else
                usedStats.Add(kvp.Key, kvp.Value);
        }
    }

    public int GetGainedAmount(CurrencyType currency)
    {
        return gainedStats.GetValueOrDefault(currency);
    }
    
    public int GetUsedAmount(CurrencyType currency)
    {
        return usedStats.GetValueOrDefault(currency);
    }
}
