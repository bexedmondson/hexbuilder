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

    public static CurrencySum operator +(CurrencySum left, CurrencySum right)
    {
        left.Add(right);
        return left;
    }
    
    public static CurrencySum operator -(CurrencySum left, CurrencySum right)
    {
        CurrencySum final = new CurrencySum();
        final.Add(left);
        
        foreach (var r in right)
        {
            if (final.ContainsKey(r.Key))
                final[r.Key] -= r.Value;
            else
                final[r.Key] = -r.Value;
        }
        
        return final;
    }

    public static CurrencySum operator *(CurrencySum operand, int multiplier)
    {
        CurrencySum final = new CurrencySum();
        foreach (var o in operand)
        {
            final[o.Key] = o.Value * multiplier;
        }
        
        return final;
    }
    
    public static bool operator ==(CurrencySum left, CurrencySum right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }
    
    public static bool operator !=(CurrencySum left, CurrencySum right)
    {
        return !(left == right);
    }

    public void Add(CurrencySum sumToAdd)
    { 
        if (sumToAdd == null)
            return;

        foreach (var kvp in sumToAdd)
        {
            Add(kvp.Key, kvp.Value);
        } 
    }
    
    public void Add(Godot.Collections.Dictionary<CurrencyType, int> sumToAdd)
    {
        if (sumToAdd == null)
            return;
        
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
        if (sumToSubtract == null)
            return;

        foreach (var kvp in sumToSubtract)
        {
            Subtract(kvp.Key, kvp.Value);
        }
    }
    
    public void Subtract(Godot.Collections.Dictionary<CurrencyType, int> sumToSubtract)
    {
        if (sumToSubtract == null)
            return;

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

    public override bool Equals(object obj)
    {
        if (obj is not CurrencySum otherSum)
            return false;

        foreach (var kvp in otherSum)
        {
            if (!this.TryGetValue(kvp.Key, out var val))
                return false;
            if (val != kvp.Value)
                return false;
        }
        
        foreach (var kvp in this)
        {
            if (!otherSum.TryGetValue(kvp.Key, out var val))
                return false;
            if (val != kvp.Value)
                return false;
        }
        
        return true;
    }

    public override int GetHashCode()
    {
        int code = 0;
        foreach (var kvp in this)
        {
            code += kvp.Key.GetHashCode();
            code += kvp.Value.GetHashCode();
        }
        return code;
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