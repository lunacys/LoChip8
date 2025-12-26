using System.Runtime.CompilerServices;

namespace LoChip8;

public static class InstructionParser
{
    private static Instruction Unknown(Interpreter interpreter, ushort instruction)
        => new(InstructionType.Unknown, "UNKN", interpreter.InstUNKN, instruction);

    public static Instruction Parse(Interpreter interpreter, ushort instruction)
    {
        return (instruction & 0xF000) switch
        {
            0x0000 => Parse0(interpreter, instruction),
            0x1000 => Parse1(interpreter, instruction),
            0x2000 => Parse2(interpreter, instruction),
            0x3000 => Parse3(interpreter, instruction),
            0x4000 => Parse4(interpreter, instruction),
            0x5000 => Parse5(interpreter, instruction),
            0x6000 => Parse6(interpreter, instruction),
            0x7000 => Parse7(interpreter, instruction),
            0x8000 => Parse8(interpreter, instruction),
            0x9000 => Parse9(interpreter, instruction),
            0xA000 => ParseA(interpreter, instruction),
            0xB000 => ParseB(interpreter, instruction),
            0xC000 => ParseC(interpreter, instruction),
            0xD000 => ParseD(interpreter, instruction),
            0xE000 => ParseE(interpreter, instruction),
            0xF000 => ParseF(interpreter, instruction),
            _ => Unknown(interpreter, instruction)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse0(Interpreter interpreter, ushort instruction)
    {
        return instruction switch
        {
            0x00E0 => new(InstructionType.I_00E0, "00E0", interpreter.Inst00E0),
            0x00EE => new(InstructionType.I_00EE, "00EE", interpreter.Inst00EE),
            _ => new(InstructionType.I_0NNN, "0NNN", interpreter.Inst0NNN, (ushort)(instruction & 0x0FFF))
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse1(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_1NNN, "1NNN", interpreter.Inst1NNN, (ushort)(instruction & 0x0FFF));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse2(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_2NNN, "2NNN", interpreter.Inst2NNN, (ushort)(instruction & 0x0FFF));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse3(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_3XNN, "3XNN", interpreter.Inst3XNN,
            (ushort)((instruction >> 8) & 0x0F),
            (ushort)(instruction & 0xFF)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse4(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_4XNN, "4XNN", interpreter.Inst4XNN,
            (ushort)((instruction >> 8) & 0x0F),
            (ushort)(instruction & 0xFF)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse5(Interpreter interpreter, ushort instruction)
    {
        if ((instruction & 0xF00F) == 0x5000)
        {
            return new(InstructionType.I_5XY0, "5XY0", interpreter.Inst5XY0,
                (ushort)((instruction >> 8) & 0x0F),
                (ushort)((instruction >> 4) & 0x0F)
            );
        }

        return Unknown(interpreter, instruction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse6(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_6XNN, "6XNN", interpreter.Inst6XNN,
            (ushort)((instruction >> 8) & 0x0F),
            (ushort)(instruction & 0xFF)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse7(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_7XNN, "7XNN", interpreter.Inst7XNN,
            (ushort)((instruction >> 8) & 0x0F),
            (ushort)(instruction & 0xFF)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse8(Interpreter interpreter, ushort instruction)
    {
        var mask = (ushort)(instruction & 0xF00F);
        var op1 = (ushort)((instruction >> 8) & 0x0F);
        var op2 = (ushort)((instruction >> 4) & 0x0F);

        if (mask == 0x8000)
        {
            return new Instruction(InstructionType.I_8XY0, "8XY0", interpreter.Inst8XY0, op1, op2);
        }

        if (mask == 0x8001)
        {
            return new Instruction(InstructionType.I_8XY1, "8XY1", interpreter.Inst8XY1, op1, op2);
        }

        if (mask == 0x8002)
        {
            return new Instruction(InstructionType.I_8XY2, "8XY2", interpreter.Inst8XY2, op1, op2);
        }

        if (mask == 0x8003)
        {
            return new Instruction(InstructionType.I_8XY3, "8XY3", interpreter.Inst8XY3, op1, op2);
        }

        if (mask == 0x8004)
        {
            return new Instruction(InstructionType.I_8XY4, "8XY4", interpreter.Inst8XY4, op1, op2);
        }

        if (mask == 0x8005)
        {
            return new Instruction(InstructionType.I_8XY5, "8XY5", interpreter.Inst8XY5, op1, op2);
        }

        if (mask == 0x8006)
        {
            return new Instruction(InstructionType.I_8XY6, "8XY6", interpreter.Inst8XY6, op1, op2);
        }

        if (mask == 0x8007)
        {
            return new Instruction(InstructionType.I_8XY7, "8XY7", interpreter.Inst8XY7, op1, op2);
        }

        if (mask == 0x800E)
        {
            return new Instruction(InstructionType.I_8XYE, "8XYE", interpreter.Inst8XYE, op1, op2);
        }

        return Unknown(interpreter, instruction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction Parse9(Interpreter interpreter, ushort instruction)
    {
        if ((instruction & 0xF00F) == 0x9000)
        {
            return new Instruction(InstructionType.I_9XY0, "9XY0", interpreter.Inst9XY0,
                (ushort)((instruction >> 8) & 0x0F),
                (ushort)((instruction >> 4) & 0x0F)
            );
        }

        return Unknown(interpreter, instruction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction ParseA(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_ANNN, "ANNN", interpreter.InstANNN, (ushort)(instruction & 0x0FFF));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction ParseB(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_BNNN, "BNNN", interpreter.InstBNNN, (ushort)(instruction & 0x0FFF));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction ParseC(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_CXNN, "CXNN", interpreter.InstCXNN,
            (ushort)((instruction >> 8) & 0x0F),
            (ushort)(instruction & 0xFF)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction ParseD(Interpreter interpreter, ushort instruction)
        => new(InstructionType.I_DXYN, "DXYN", interpreter.InstDXYN,
            (ushort)((instruction >> 8) & 0x0F),
            (ushort)((instruction >> 4) & 0x0F),
            (ushort)(instruction & 0x0F)
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction ParseE(Interpreter interpreter, ushort instruction)
    {
        var mask = (ushort)(instruction & 0xF0FF);
        var op = (ushort)((instruction >> 8) & 0x0F);

        if (mask == 0xE09E)
        {
            return new Instruction(InstructionType.I_EX9E, "EX9E", interpreter.InstEX9E, op);
        }

        if (mask == 0xE0A1)
        {
            return new Instruction(InstructionType.I_EXA1, "EXA1", interpreter.InstEXA1, op);
        }

        return Unknown(interpreter, instruction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Instruction ParseF(Interpreter interpreter, ushort instruction)
    {
        var mask = instruction & 0xF0FF;
        var op = (ushort)((instruction >> 8) & 0x0F);

        if (mask == 0xF007)
        {
            return new Instruction(InstructionType.I_FX07, "FX07", interpreter.InstFX07, op);
        }

        if (mask == 0xF00A)
        {
            return new Instruction(InstructionType.I_FX0A, "FX0A", interpreter.InstFX0A, op);
        }

        if (mask == 0xF015)
        {
            return new Instruction(InstructionType.I_FX15, "FX15", interpreter.InstFX15, op);
        }

        if (mask == 0xF018)
        {
            return new Instruction(InstructionType.I_FX18, "FX18", interpreter.InstFX18, op);
        }

        if (mask == 0xF01E)
        {
            return new Instruction(InstructionType.I_FX1E, "FX1E", interpreter.InstFX1E, op);
        }

        if (mask == 0xF029)
        {
            return new Instruction(InstructionType.I_FX29, "FX29", interpreter.InstFX29, op);
        }

        if (mask == 0xF033)
        {
            return new Instruction(InstructionType.I_FX33, "FX33", interpreter.InstFX33, op);
        }

        if (mask == 0xF055)
        {
            return new Instruction(InstructionType.I_FX55, "FX55", interpreter.InstFX55, op);
        }

        if (mask == 0xF065)
        {
            return new Instruction(InstructionType.I_FX65, "FX65", interpreter.InstFX65, op);
        }

        return Unknown(interpreter, instruction);
    }
}