namespace LoChip8;

// ReSharper disable InconsistentNaming
public enum InstructionType
{
    Unknown, 
    I_0NNN, I_00E0, I_00EE, 
    I_1NNN, I_2NNN, I_3XNN, I_4XNN, I_5XY0, I_6XNN, I_7XNN, 
    I_8XY0, I_8XY1, I_8XY2, I_8XY3, I_8XY4, I_8XY5, I_8XY6, I_8XY7, I_8XYE, 
    I_9XY0, I_ANNN, I_BNNN, I_CXNN, I_DXYN, 
    I_EX9E, I_EXA1, 
    I_FX07, I_FX0A, I_FX15, I_FX18, I_FX1E, I_FX29, I_FX33, I_FX55, I_FX65
}
// ReSharper restore InconsistentNaming

public delegate void Operate(in Instruction instruction);

public readonly ref struct Instruction(
    InstructionType type,
    string name,
    Operate operation,
    ushort op1 = 0,
    ushort op2 = 0,
    ushort op3 = 0
)
{
    public readonly InstructionType Type = type;
    public readonly string Name = name;
    public readonly Operate Operation = operation;
    public readonly ushort Op1 = op1;
    public readonly ushort Op2 = op2;
    public readonly ushort Op3 = op3;
}