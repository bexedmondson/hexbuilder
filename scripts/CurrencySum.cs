using System.Collections.Generic;

public class CurrencySum : Dictionary<CurrencyType, int>
{
    public CurrencySum() : base() { }
    public CurrencySum(Godot.Collections.Dictionary<CurrencyType, int> godotDict) : base(godotDict) { }
    
    public void Add(CurrencySum sumToAdd)
    {
        foreach (var kvp in sumToAdd)
        {
            if (this.ContainsKey(kvp.Key))
                this[kvp.Key] += kvp.Value;
            else
                this[kvp.Key] = kvp.Value;
        }
    }
}