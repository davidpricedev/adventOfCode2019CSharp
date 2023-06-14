namespace AdventOfCode2019CSharp.util;

public static class Extensions
{
    public static IEnumerable<string> ToCharsAsStr(this string str) => str.ToCharArray().Select(x => x.ToString());
}