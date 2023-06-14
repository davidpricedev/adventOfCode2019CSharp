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
    [Fact]
    public void WillHandleMixedModeMultiplication()
    {
        var program = new List<long>() { 1002, 4, 3, 4, 33 };
        var result = ComputerFactory.StartComputer(program, new List<long>());
        Assert.Equal(99L, result.Memory[4]);
    }
    [Fact]
    public void WillHandleMixedModeSubtraction()
    {
        var program = new List<long>() { 1101, 100, -1, 4, 0 };
        var result = ComputerFactory.StartComputer(program, new List<long>());
        Assert.Equal(99L, result.Memory[4]);
    }
}