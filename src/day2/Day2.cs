using AdventOfCode2019CSharp.iccomp;
using AdventOfCode2019CSharp.util;

namespace AdventOfCode2019CSharp.day2;

public static class Day2
{
    public static void Run()
    {
        Aoc.RunPart(2, 1, Part1);
        Aoc.RunPart(2, 2, Part2);
    }

    static double Part1(IEnumerable<string> lines) =>
        RunComputer(GetProgram(lines, 12, 2));

    static double Part2(IEnumerable<string> lines)
    {
        const long targetOutput = 19690720L;
        foreach (var x in Enumerable.Range(0, 99))
        {
            var rawProgram = lines.ToList();
            foreach (var y in Enumerable.Range(0, 99))
            {
                var result = RunComputer(GetProgram(rawProgram, x, y));
                if (result == targetOutput)
                {
                    Aoc.Println($"{x}, {y} -> {result}");
                    return 100 * x + y;
                }
            }
        }

        return -1;
    }

    static long RunComputer(List<long> program) =>
        ComputerFactory.StartComputer(program, new List<long>()).Memory[0];

    static List<long> GetProgram(IEnumerable<string> lines, int noun, int verb) =>
        ComputerFactory.InputStringToList(lines.First()).Select((x, i) => i switch
        {
            1 => noun,
            2 => verb,
            _ => x
        }).ToList();
}