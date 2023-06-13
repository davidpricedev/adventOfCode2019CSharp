using System.CodeDom.Compiler;

namespace AdventOfCode2019CSharp.util;

/**
 * An implementation that is somewhat similar to Kotlin's generateSequence function
 */
public static class Sequence
{
    public static IEnumerable<T> Generate<T>(T seed, Func<T, T> nextFn, Func<T, bool> continuePredicate)
    {
        var state = seed;
        do
        {
            yield return state;
            state = nextFn(state);
        } while (!continuePredicate(state));
    }
}