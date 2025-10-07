using System.Collections.Generic;
using System.Text;

public class CurrencySum : Dictionary<CurrencyType, int>
{
    public CurrencySum() : base() { }
    public CurrencySum(Godot.Collections.Dictionary<CurrencyType, int> godotDict) : base(godotDict) { }
    
    public CurrencySum(CurrencyType initialCurrency, int initialAmount = 0)
    {
        this.Add(initialCurrency, initialAmount);
    }
    
    public void Add(CurrencySum sumToAdd)
    { 
        foreach (var kvp in sumToAdd)
        {
            Add(kvp.Key, kvp.Value);
        } 
    }
    
    public void Add(Godot.Collections.Dictionary<CurrencyType, int> sumToAdd)
    {
        foreach (var kvp in sumToAdd)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    public void Add(CurrencyType currencyType, int delta)
    {
        if (this.ContainsKey(currencyType))
            this[currencyType] += delta;
        else
            this[currencyType] = delta;
    }
    
    public void Subtract(CurrencySum sumToSubtract)
    {
        foreach (var kvp in sumToSubtract)
        {
            Subtract(kvp.Key, kvp.Value);
        }
    }
    
    public void Subtract(Godot.Collections.Dictionary<CurrencyType, int> sumToSubtract)
    {
        foreach (var kvp in sumToSubtract)
        {
            Subtract(kvp.Key, kvp.Value);
        }
    }
    
    public void Subtract(CurrencyType currencyType, int delta)
    {
        if (this.ContainsKey(currencyType))
            this[currencyType] -= delta;
        else
            this[currencyType] = -delta;
    }

    public CurrencySum Get(CurrencyType currencyType)
    {
        return TryGetValue(currencyType, out int amount) ? new CurrencySum(currencyType, amount) : new CurrencySum(currencyType);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("{");
        foreach (var kvp in this)
        {
            sb.Append($" {kvp.Key.ToString()}: {kvp.Value},");
        }
        sb.Append(" }");
        return sb.ToString();
    }
}