using System.Diagnostics.CodeAnalysis;
using AdventOfCode2019CSharp.iccomp;

namespace tests;

public class ComputerTest
{
    [Fact]
    public void WillHandleSimpleAddition()
    {
        var program = new List<long>() { 1, 0, 0, 0, 99 };
        var result = ComputerFactory.StartComputer(program, new List<long>());
        Assert.Equal(2L, result.Memory[0]);
    }
}