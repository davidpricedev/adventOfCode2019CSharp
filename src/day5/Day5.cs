using AdventOfCode2019CSharp.iccomp;
using AdventOfCode2019CSharp.util;

namespace AdventOfCode2019CSharp.day5;

/**
 * Mostly complete the intcode computer
 * part 1 needs implementation of inputs and outputs and the position/immediate mode modifiers for opcodes
 * part 2 needs implementation of jump-if-true, jump-if-false, less-than, and equals
 * At this point we are starting to use inputs and outputs
 */
public static class Day5
{
    private const int DayNumber = 5;

    public static void Run()
    {
        Aoc.RunPart(DayNumber, 1, Part1);
        Aoc.RunPart(DayNumber, 2, Part2);
    }

    static double Part1(IEnumerable<string> lines) => RunComputer(
        ComputerFactory.InputStringToList(lines.First()), 1L
    );

    static double Part2(IEnumerable<string> lines) => RunComputer(
        ComputerFactory.InputStringToList(lines.First()), 5L
    );

    static long RunComputer(List<long> program, long input) =>
        ComputerFactory.RunComputer(program, new List<long>() { input }).LastOrDefault();
}