namespace AdventOfCode2019CSharp.util;

public class Aoc
{
    public static void RunPart(int day, int part, Func<IEnumerable<string>, double> partFunc) => 
        Console.WriteLine($"part{part} result: {partFunc(ReadLocalLines($"day{day}"))}");
    
    // Jump out of the bin/Debug/net7.0/dayN path into the root of the project
    public const string JumpOut = "../../../";
    
    public const string TableFlipString = "(╯°□°)╯︵ ┻━┻";
    
    public static string ReadAll(string filename) => File.ReadAllText(filename); 

    public static string[] ReadLines(string filename) => File.ReadAllLines(filename); 
    
    public static string DayInputFilename(string day) => Path.Combine(Environment.CurrentDirectory, JumpOut, day, "input.txt");

    public static string[] ReadLocalLines(string day) => ReadLines(DayInputFilename(day));

    public static void Println(string msg) => Console.WriteLine(msg);

    public static string TableFlip(string? msg = null) => msg?.Length > 0 ? $"{TableFlipString} {msg}" : TableFlipString;
}