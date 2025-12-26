using LoChip8.Logging;

namespace LoChip8;

public class Interpreter
{
    public ushort RegI;
    public byte DelayTimer;
    public byte SoundTimer;
    public ushort RegPc { get; private set; }
    public byte RegSp { get; private set; }
    
    public readonly byte[] DataRegisters = new byte[16];
    public readonly ushort[] Stack = new ushort[16];

    public bool IsWaitingForKeyPress;
    public byte RegisterToStoreKeyPressed;
    
    public Ram Ram { get; }
    public Display Display { get; }
    public Keypad Keypad { get; }
    public IBeeper? Beeper { get; }

    private readonly Random _rng = new();

    public Interpreter(IBeeper? beeper = null)
    {
        Ram = new Ram();
        Display = new Display();
        Keypad = new Keypad();
        Beeper = beeper;
        
        RegPc = Ram.LoadingAddress;

        Keypad.KeyPressed += (_, e) =>
        {
            if (IsWaitingForKeyPress)
            {
                IsWaitingForKeyPress = false;
                DataRegisters[RegisterToStoreKeyPressed] = e;
            }
        };
    }

    public void Clock()
    {
        var instrHi = Ram.Read(RegPc);
        RegPc++;
        var instrLo = Ram.Read(RegPc);
        RegPc++;
        
        ushort instr = (ushort)((instrHi << 8) | instrLo);
        var parsed = InstructionParser.Parse(this, instr);
        parsed.Operation(parsed);

        if (DelayTimer > 0) DelayTimer--;
        if (SoundTimer > 0)
        {
            Beeper?.Beep(1);
            SoundTimer--;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < DataRegisters.Length; i++)
        {
            DataRegisters[i] = 0;
        }

        for (int i = 0; i < Stack.Length; i++)
        {
            Stack[i] = 0;
        }
        
        Ram.Reset();
        Keypad.Reset();

        RegI = 0;
        RegPc = Ram.LoadingAddress;
        RegSp = 0;
        DelayTimer = 0;
        SoundTimer = 0;
    }
    
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Execute machine language subroutine at address NNN
    /// </summary>
    internal void Inst0NNN(in Instruction instruction)
    {
        Log.Warning("Tried to use the 0NNN instruction", "Interpreter");
    }
    
    /// <summary>
    /// Clear the screen
    /// </summary>
    internal void Inst00E0(in Instruction instruction)
    {
        Display.Clear();
    }

    /// <summary>
    /// Return from a subroutine
    /// </summary>
    internal void Inst00EE(in Instruction instruction)
    {
        RegPc = Stack[RegSp--];
    }

    /// <summary>
    /// Jump to address NNN
    /// </summary>
    internal void Inst1NNN(in Instruction instruction)
    {
        RegPc = instruction.Op1;
    }

    /// <summary>
    /// Execute subroutine at address NNN
    /// </summary>
    internal void Inst2NNN(in Instruction instruction)
    {
        Stack[++RegSp] = RegPc;
        RegPc = instruction.Op1;
    }

    /// <summary>
    /// Skip the following instruction if the value of register VX equals NN
    /// </summary>
    internal void Inst3XNN(in Instruction instruction)
    {
        if (DataRegisters[instruction.Op1] == instruction.Op2)
            RegPc += 2;
    }

    /// <summary>
    /// Skip the following instruction if the value of register VX is not equal to NN
    /// </summary>
    internal void Inst4XNN(in Instruction instruction)
    {
        if (DataRegisters[instruction.Op1] != instruction.Op2)
            RegPc += 2;
    }

    /// <summary>
    /// Skip the following instruction if the value of register VX is equal to the value of register VY
    /// </summary>
    internal void Inst5XY0(in Instruction instruction)
    {
        if (DataRegisters[instruction.Op1] == DataRegisters[instruction.Op2])
            RegPc += 2;
    }

    /// <summary>
    /// Store number NN in register VX
    /// </summary>
    internal void Inst6XNN(in Instruction instruction)
    {
        DataRegisters[instruction.Op1] = (byte) instruction.Op2;
    }
    
    /// <summary>
    /// Add the value NN to register VX. Overflow (carry) is ignored.
    /// </summary>
    internal void Inst7XNN(in Instruction instruction)
    {
        unchecked
        {
            DataRegisters[instruction.Op1] += (byte)instruction.Op2;
        }
    }

    /// <summary>
    /// Store the value of register VY in register VX
    /// </summary>
    internal void Inst8XY0(in Instruction instruction)
    {
        DataRegisters[instruction.Op1] = DataRegisters[instruction.Op2];
    }

    /// <summary>
    /// Set VX to VX | VY
    /// </summary>
    internal void Inst8XY1(in Instruction instruction)
    {
        var reg1 = DataRegisters[instruction.Op1];
        var reg2 = DataRegisters[instruction.Op2];

        DataRegisters[instruction.Op1] = (byte)(reg1 | reg2);
    }

    /// <summary>
    /// Set VX to VX & VY
    /// </summary>
    internal void Inst8XY2(in Instruction instruction)
    {
        var reg1 = DataRegisters[instruction.Op1];
        var reg2 = DataRegisters[instruction.Op2];

        DataRegisters[instruction.Op1] = (byte)(reg1 & reg2);
    }
    
    /// <summary>
    /// Set VX to VX ^ VY
    /// </summary>
    internal void Inst8XY3(in Instruction instruction)
    {
        var reg1 = DataRegisters[instruction.Op1];
        var reg2 = DataRegisters[instruction.Op2];

        DataRegisters[instruction.Op1] = (byte)(reg1 ^ reg2);
    }
    
    /// <summary>
    /// Add the value of register VY to register VX.
    /// Set VF to 01 if a carry occurs.
    /// Set VF to 00 if a carry does not occur.
    /// </summary>
    internal void Inst8XY4(in Instruction instruction)
    {
        var yVal = DataRegisters[instruction.Op2];

        var x = instruction.Op1;
        var xVal = DataRegisters[x];

        var sum = (ushort)(xVal + yVal);
        DataRegisters[x] = (byte)sum;
        DataRegisters[0xF] = (byte)((sum > 0xFF) ? 1 : 0);
    }
    
    /// <summary>
    /// Subtract the value of register VY from register VX.
    /// Set VF to 00 if a borrow occurs.
    /// Set VF to 01 if a borrow does not occur.
    /// </summary>
    internal void Inst8XY5(in Instruction instruction)
    {
        var yVal = DataRegisters[instruction.Op2];
        
        var x = instruction.Op1;
        var xVal = DataRegisters[x];

        unchecked
        {
            DataRegisters[x] -= yVal;
        }

        if (xVal < yVal)
            DataRegisters[0xF] = 0x01;
        else
            DataRegisters[0xF] = 0x00;
    }
    
    /// <summary>
    /// Store the value of register VY shifted right one bit in register VX.
    /// Set register VF to the least significant bit prior to the shift.
    /// VY is unchanged.
    /// </summary>
    internal void Inst8XY6(in Instruction instruction)
    {
        var xReg = instruction.Op1;
        var yVal = DataRegisters[instruction.Op2];

        var lsb = yVal & 1;

        DataRegisters[xReg] = (byte)(yVal >> 1);
        DataRegisters[0xF] = (byte)lsb;
    }
    
    /// <summary>
    /// Set register VX to the value of VY minus VX.
    /// Set VF to 00 if a borrow occurs.
    /// Set VF to 01 if a borrow does not occur.
    /// </summary>
    internal void Inst8XY7(in Instruction instruction)
    {
        var yVal = DataRegisters[instruction.Op2];

        var x = instruction.Op1;
        var xVal = DataRegisters[x];

        unchecked
        {
            DataRegisters[x] = (byte)(yVal - xVal);
        }
        
        if (yVal >= xVal)
            DataRegisters[0xF] = 0x00;
        else 
            DataRegisters[0xF] = 0x01;
    }

    /// <summary>
    /// Store the value of register VY shifted left one bit in register VX.
    /// Set register VF to the most significant bit prior to the shift.
    /// VY is unchanged.
    /// </summary>
    internal void Inst8XYE(in Instruction instruction)
    {
        var xReg = instruction.Op1;
        var yVal = DataRegisters[instruction.Op2];

        var msb = (yVal >> 7) & 0x01;
        
        DataRegisters[xReg] = (byte)(yVal << 1);
        DataRegisters[0xF] = (byte)msb;
    }

    /// <summary>
    /// Skip the following instruction if the value of register VX is not equal to the value of register VY
    /// </summary>
    internal void Inst9XY0(in Instruction instruction)
    {
        if (DataRegisters[instruction.Op1] != DataRegisters[instruction.Op2])
            RegPc += 2;
    }

    /// <summary>
    /// Store memory address NNN in register I
    /// </summary>
    internal void InstANNN(in Instruction instruction)
    {
        RegI = instruction.Op1;
    }

    /// <summary>
    /// Jump to address NNN + V0
    /// </summary>
    internal void InstBNNN(in Instruction instruction)
    {
        RegPc = (ushort)(instruction.Op1 + DataRegisters[0]);
    }

    /// <summary>
    /// Set VX to a random number with a mask of NN
    /// </summary>
    internal void InstCXNN(in Instruction instruction)
    {
        var num = (byte)_rng.Next(256);
        var mask = instruction.Op2;

        DataRegisters[instruction.Op1] = (byte)(num & mask);
    }

    /// <summary>
    /// Draw a sprite at position VX, VY with N bytes of sprite data starting at the address stored in I.
    /// Set VF to 01 if any set pixels are changed to unset, and 00 otherwise.
    /// </summary>
    internal void InstDXYN(in Instruction instruction)
    {
        var x = DataRegisters[instruction.Op1];
        var y = DataRegisters[instruction.Op2];
        var n = instruction.Op3;

        byte collision = Display.DrawSprite(Ram, RegI, x, y, n);
        DataRegisters[0xF] = collision;
    }

    /// <summary>
    /// Skip the following instruction if the key corresponding to the hex value currently stored in register VX is pressed
    /// </summary>
    internal void InstEX9E(in Instruction instruction)
    {
        var key = DataRegisters[instruction.Op1];
        if (Keypad.IsKeyDown(key))
            RegPc += 2;
    }

    /// <summary>
    /// Skip the following instruction if the key corresponding to the hex value currently stored in register VX is not pressed
    /// </summary>
    internal void InstEXA1(in Instruction instruction)
    {
        var key = DataRegisters[instruction.Op1];
        if (!Keypad.IsKeyDown(key))
            RegPc += 2;
    }

    /// <summary>
    /// Store the current value of the delay timer in register VX
    /// </summary>
    internal void InstFX07(in Instruction instruction)
    {
        DataRegisters[instruction.Op1] = DelayTimer;
    }

    /// <summary>
    /// Wait for a keypress and store the result in register VX.
    /// All execution stops until a key is pressed, then the value of that key is stored in VX.
    /// </summary>
    internal void InstFX0A(in Instruction instruction)
    {
        IsWaitingForKeyPress = true;
        RegisterToStoreKeyPressed = (byte)instruction.Op1;
    }

    /// <summary>
    /// Set the delay timer to the value of register VX
    /// </summary>
    internal void InstFX15(in Instruction instruction)
    {
        DelayTimer = DataRegisters[instruction.Op1];
    }

    /// <summary>
    /// Set the sound timer to the value of register VX
    /// </summary>
    internal void InstFX18(in Instruction instruction)
    {
        SoundTimer = DataRegisters[instruction.Op1];
    }

    /// <summary>
    /// Add the value stored in register VX to register I
    /// </summary>
    internal void InstFX1E(in Instruction instruction)
    {
        RegI += DataRegisters[instruction.Op1];
    }

    /// <summary>
    /// Set I to the memory address of the sprite data corresponding to the hexadecimal digit stored in register VX
    /// </summary>
    internal void InstFX29(in Instruction instruction)
    {
        RegI = Ram.GetDefaultSpriteAddress(DataRegisters[instruction.Op1]);
    }

    /// <summary>
    /// Store the binary-coded decimal equivalent of the value stored in register VX at addresses I, I + 1, and I + 2.
    /// The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I,
    /// the tens digit at location I+1, and the ones digit at location I+2.
    /// </summary>
    internal void InstFX33(in Instruction instruction)
    {
        var value = DataRegisters[instruction.Op1];
        var hundreds = value / 100;
        value %= 100;
        var tens = value / 10;
        value %= 10;

        // TODO: Check if needed to increment the I register.
        Ram.Write(RegI++, (byte)hundreds);
        Ram.Write(RegI++, (byte)tens);
        Ram.Write(RegI++, value);
    }

    /// <summary>
    /// Store the values of registers V0 to VX inclusive in memory starting at address I.
    /// I is set to I + X + 1 after operation.
    /// </summary>
    internal void InstFX55(in Instruction instruction)
    {
        //var vx = DataRegisters[instruction.Op1];
        var vx = (byte)instruction.Op1; // TODO: Check this

        for (int i = 0; i < vx; i++)
        {
            Ram.Write((ushort)(RegI + i), DataRegisters[i]);
        }

        RegI += (ushort)(vx + 1);
    }

    /// <summary>
    /// Fill registers V0 to VX inclusive with the values stored in memory starting at address I.
    /// I is set to I + X + 1 after operation.
    /// </summary>
    internal void InstFX65(in Instruction instruction)
    {
        //var vx = DataRegisters[instruction.Op1];
        var vx = (byte)instruction.Op1;

        for (int i = 0; i < vx; i++)
        {
            var val = Ram.Read((ushort)(RegI + i));
            DataRegisters[i] = val;
        }

        RegI += (ushort)(vx + 1);
    }

    /// <summary>
    /// An unknown instruction
    /// </summary>
    internal void InstUNKN(in Instruction instruction)
    {
        Log.Warning("Executing unknown instruction", "Interpreter");
    }
    // ReSharper enable InconsistentNaming
}