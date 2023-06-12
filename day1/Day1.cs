using System.Diagnostics.CodeAnalysis;
using AdventOfCode2019CSharp.util;

namespace AdventOfCode2019CSharp.day1;

public static class Day1
{
    public static void Run()
    {
        Aoc.RunPart(1, 1, Part1);
        Aoc.RunPart(1, 2, Part2);
    }

    static double Part1(IEnumerable<string> inputs) => inputs.Select(x => MassToFuelNaive(Numeric.ToDouble(x))).Sum();

    static double MassToFuelNaive(double mass) => Math.Floor(mass / 3) - 2;

    static double Part2(IEnumerable<string> inputs) => inputs.Select(x => MassToFuel(Numeric.ToDouble(x))).Sum();

    static double MassToFuel(double mass) => CalculateFuelForFuelMass(MassToFuelNaive(mass));

    static double CalculateFuelForFuelMass(double initialFuelMass)
    {
        var sumSoFar = 0.0;
        var fuelMass = initialFuelMass;
        while (fuelMass > 0)
        {
            sumSoFar += fuelMass;
            fuelMass = MassToFuelNaive(fuelMass);
        }

        return sumSoFar;
    }
}
