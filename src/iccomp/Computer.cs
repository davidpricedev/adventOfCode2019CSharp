using System.Diagnostics;
using AdventOfCode2019CSharp.util;

namespace AdventOfCode2019CSharp.iccomp;

public static class ComputerFactory
{
    private const int DefaultMemSize = 2048;

    public static List<long> InputStringToList(string inputStr) =>
        inputStr.Split(",").Select(long.Parse).ToList();

    public static IEnumerable<long> RunComputer(string programRaw, List<long>? inputs = null, bool debug = false) =>
        RunComputer(InputStringToList(programRaw), inputs ?? new List<long>(), debug);

    /**
     * Start the computer, run it until it stops for any reason, return the outputs
     */
    public static IEnumerable<long> RunComputer(List<long> program, List<long> inputs, bool debug = false) =>
        StartComputer(program, inputs, debug).Outputs;

    /**
     * Start the computer and run it until it stops for any reason and return the computer at that point
     */
    public static Computer StartComputer(List<long> program, List<long> inputs, bool debug = false) =>
        new Computer(Memory: InitializeMemory(program), Inputs: inputs, Outputs: new List<long>(), Debug: debug)
            .RunUntilPrompt();

    private static List<long> InitializeMemory(IReadOnlyCollection<long> program)
    {
        // Fill memory beyond the program with zeroes
        var memPad = Enumerable.Range(0, (DefaultMemSize - program.Count)).Select(x => 0L);
        if (program.Count > DefaultMemSize)
            throw new Exception("program too big for current computer memory");

        return program.Concat(memPad).ToList();
    }
}

public enum ComputerStatus
{
    Ready,
    Running,
    Halted,
    WaitingForInput
}

public record class Computer(
    List<long> Memory,
    List<long> Inputs,
    List<long> Outputs,
    bool Debug = false,
    int CurrentPos = 0,
    int CurrentRelBase = 0,
    int InputPtr = 0,
    ComputerStatus Status = ComputerStatus.Running
)
{
    private int CurrentInstruction() => (int)Memory[CurrentPos];

    private int CurrentOpCode() => CurrentInstruction() - (100 * (CurrentInstruction() / 100));

    public long ValueAt(int position) => Memory[position];

    public long ValueAt(long position) => Memory[(int)position];

    private void DebugMessage(string message)
    {
        if (this.Debug)
            Aoc.Println(message);
    }

    public Computer RunNext()
    {
        DebugMessage(this.ToString());
        return ApplyOpcode();
    }

    public Computer RunUntilPrompt()
    {
        var sequence = Sequence.Generate(this, x => x.RunNext(), x => x.Status == ComputerStatus.Running);
        return sequence.LastOrDefault() ?? this;
    }

    public Tuple<Computer, List<long>> RunInput(IEnumerable<long> moreInputs)
    {
        var withNewInputs = this with { Inputs = Inputs.Concat(moreInputs).ToList() };
        var finalState = withNewInputs.RunUntilPrompt();
        var outputs = finalState.Outputs.TakeLast(finalState.Outputs.Count - Outputs.Count).ToList();
        return new Tuple<Computer, List<long>>(finalState, outputs);
    }

    private Computer CopyWithNewValueAt(int position, long newValue)
    {
        var newMemory = Memory.Select((x, index) => index == position ? newValue : x).ToList();
        return this with { Memory = newMemory, CurrentPos = CurrentPos + OpcodeLength(CurrentOpCode()) };
    }

    public string MemoryAsString() => string.Join(",", Memory);

    private long GetInParam(int paramOrd)
    {
        var mode = GetParamModes()[paramOrd - 1];
        return mode switch
        {
            0 => ValueAt(ValueAt(CurrentPos + paramOrd)),
            1 => ValueAt(CurrentPos + paramOrd),
            2 => ValueAt(CurrentRelBase + ValueAt(CurrentPos + paramOrd)),
            _ => throw new Exception($"{Aoc.TableFlip()} parameter mode {mode} is unknown")
        };
    }

    private int GetOutPos(int paramOrd)
    {
        var mode = GetParamModes()[paramOrd - 1];
        return mode switch
        {
            0 => (int)ValueAt(CurrentPos + paramOrd),
            2 => CurrentRelBase + (int)ValueAt(CurrentPos + paramOrd),
            _ => throw new Exception($"{Aoc.TableFlip()} parameter mode {mode} for result param is unexpected")
        };
    }

    private Computer ApplyOpcode()
    {
        var opCode = CurrentOpCode();
        return opCode switch
        {
            1 => ApplyAdd(),
            2 => ApplyMultiply(),
            3 => ApplyInput(),
            4 => ApplyOutput(),
            5 => ApplyJumpIfTrue(),
            6 => ApplyJumpIfFalse(),
            7 => ApplyLessThan(),
            8 => ApplyEqual(),
            9 => ApplyChangeRelBase(),
            99 => ApplyHalt(),
            _ => throw new Exception($"{Aoc.TableFlip()}opcode {opCode} is unknown"),
        };
    }

    private int OpcodeLength(int opcode) =>
        opcode switch
        {
            1 => 4,
            2 => 4,
            7 => 4,
            8 => 4,
            3 => 2,
            4 => 2,
            9 => 2,
            5 => 3,
            6 => 3,
            99 => 1,
            _ => throw new Exception($"{Aoc.TableFlip()} opcode {opcode} is unknown")
        };

    private List<int> GetParamModes()
    {
        var instruction = CurrentInstruction();
        var modeString = (instruction / 100).ToString().PadLeft(3, '0');
        return modeString.ToCharsAsStr().Select(int.Parse).Reverse().ToList();
    }

    private Computer ApplyAdd()
    {
        var x = GetInParam(1);
        var y = GetInParam(2);
        var outaddr = GetOutPos(3);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] adding {x} to {y}, result to &{outaddr}");
        return CopyWithNewValueAt(outaddr, x + y);
    }

    private Computer ApplyMultiply()
    {
        var x = GetInParam(1);
        var y = GetInParam(2);
        var outaddr = GetOutPos(3);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] multiplying {x} to {y}, result to &{outaddr}");
        return CopyWithNewValueAt(outaddr, x * y);
    }

    private Computer ApplyInput()
    {
        if (Inputs.Count == 0 || InputPtr >= Inputs.Count)
            return this with { Status = ComputerStatus.WaitingForInput };
        var outaddr = GetOutPos(1);
        var inputVal = Inputs[InputPtr];
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] inputing {inputVal} result to &{outaddr}");
        var newInputs = Inputs.ToList();
        newInputs[InputPtr] = inputVal;
        return CopyWithNewValueAt(outaddr, inputVal) with { InputPtr = InputPtr + 1 };
    }

    private Computer ApplyOutput()
    {
        var outval = GetInParam(1);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] outputing {outval}");
        return this with
        {
            CurrentPos = CurrentPos + OpcodeLength(CurrentOpCode()),
            Outputs = Outputs.Concat(new List<long>() { outval }).ToList()
        };
    }

    private Computer ApplyJumpIfTrue()
    {
        var checkVal = GetInParam(1);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] jump-if-true {checkVal}");
        return checkVal != 0 ? ApplyJump() : AdvancePastJump();
    }

    private Computer ApplyJumpIfFalse()
    {
        var checkVal = GetInParam(1);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] jump-if-false {checkVal}");
        return checkVal == 0 ? ApplyJump() : AdvancePastJump();
    }

    private Computer AdvancePastJump()
    {
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] jump-check failed");
        return this with { CurrentPos = CurrentPos + OpcodeLength(CurrentOpCode()) };
    }

    private Computer ApplyJump()
    {
        var jumpTarget = (int)GetInParam(2);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] jumping to &{jumpTarget}");
        return this with { CurrentPos = jumpTarget };
    }

    private Computer ApplyLessThan()
    {
        var x = GetInParam(1);
        var y = GetInParam(2);
        var outaddr = GetOutPos(3);
        var result = x < y ? 1L : 0L;
        DebugMessage(
            $"[{CurrentPos}, {CurrentRelBase}] lessthan-check result {result}, storing result to &{outaddr}");
        return CopyWithNewValueAt(outaddr, result);
    }

    private Computer ApplyEqual()
    {
        var x = GetInParam(1);
        var y = GetInParam(2);
        var outaddr = GetOutPos(3);
        var result = x == y ? 1L : 0L;
        DebugMessage(
            $"[{CurrentPos}, {CurrentRelBase}] equal-check result {result}, storing result to &{outaddr}");
        return CopyWithNewValueAt(outaddr, result);
    }

    private Computer ApplyChangeRelBase()
    {
        var offset = (int)GetInParam(1);
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] changing relative base by {offset}");
        return this with
        {
            CurrentRelBase = CurrentRelBase + offset, CurrentPos = CurrentPos + OpcodeLength(CurrentOpCode())
        };
    }

    private Computer ApplyHalt()
    {
        DebugMessage($"[{CurrentPos}, {CurrentRelBase}] halting");
        return this with { Status = ComputerStatus.Halted };
    }
}