namespace AdventOfCode2019CSharp.util;


public static class Numeric
{
    public static int ToInt(string valueAsString)
    {
        var result = int.TryParse(valueAsString, out var x);
        return result ? x : default(int);
    }

    public static long ToLong(string valueAsString)
    {
        var result = long.TryParse(valueAsString, out var x);
        return result ? x : default(long);
    }
    
    public static double ToDouble(string valueAsString)
    {
        var result = double.TryParse(valueAsString, out var x);
        return result ? x : default(double);
    }
}