using System;

[Flags]
public enum Comparison
{
    None = 0,
    Equal,
    LessThan,
    MoreThan
}

public static class ComparisonExtensions
{
    public static bool IsPass(this int testValue, Comparison comparison, int threshold)
    {
        if (comparison == Comparison.None && testValue != threshold)
            return true;
        if (comparison.HasFlag(Comparison.Equal) && testValue == threshold)
            return true;
        if (comparison.HasFlag(Comparison.LessThan) && testValue < threshold)
            return true;
        if (comparison.HasFlag(Comparison.MoreThan) && testValue > threshold)
            return true;
        return false;
    }
}