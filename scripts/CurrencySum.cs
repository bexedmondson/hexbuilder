using System.Collections.Generic;
using System.Text;
using Godot;

public class CurrencySum : Dictionary<CurrencyType, int>
{
    public CurrencySum() : base() { }
    public CurrencySum(Godot.Collections.Dictionary<CurrencyType, int> godotDict) : base(godotDict) { }
    
    public void Add(CurrencySum sumToAdd)
    {
        GD.Print(this);
        foreach (var kvp in sumToAdd)
        {
            if (this.ContainsKey(kvp.Key))
                this[kvp.Key] += kvp.Value;
            else
                this[kvp.Key] = kvp.Value;
        }
        GD.Print(this);
    }
    
    public void Add(Godot.Collections.Dictionary<CurrencyType, int> sumToAdd)
    {
        foreach (var kvp in sumToAdd)
        {
            if (this.ContainsKey(kvp.Key))
                this[kvp.Key] += kvp.Value;
            else
                this[kvp.Key] = kvp.Value;
        }
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