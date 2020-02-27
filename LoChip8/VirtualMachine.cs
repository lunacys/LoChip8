using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LoChip8
{
    public class VirtualMachine
    {
        public static ushort LoadingAddress => 0x200;
        
        public IBeeper Beeper { get; }
        public IKeyboardProvider Keyboard { get; }
        public IDisplay Display { get; }
        
        private byte[] _ram = new byte[4096]; // 4096 bytes (4KB)
        
        private byte[] _registers = new byte[16]; // General-purpose registers Vx (where x is from 0x0 to 0xF)
        private ushort _registerI;
        private ushort _registerPC; // Program Counter
        private byte _registerSP;  // Stack Pointer
        
        private ushort[] _stack = new ushort[16];

        private byte _registerDT; // Delay Timer
        private byte _registerST; // Sound Timer

        private int _loadedRomSize;

        private bool _isWaitingForKeyPress = false;

        public VirtualMachine(IBeeper beeper, IKeyboardProvider keyboard, IDisplay display)
        {
            Beeper = beeper;
            Keyboard = keyboard;
            Display = display;
        }

        public void Initialize()
        {
            var defaultSprites = Sprite.GenerateDefaultSprites();

            for (var i = 0; i < _ram.Length; i++)
                _ram[i] = 0;
            for (var i = 0; i < _registers.Length; i++)
                _registers[i] = 0;
            for (var i = 0; i < _stack.Length; i++)
                _stack[i] = 0;

            _registerI = 0;
            _registerPC = 0;
            _registerSP = 0;
            _registerDT = 0;
            _registerST = 0;

            for (int i = 0; i < defaultSprites.Count; i++)
            {
                var sprite = defaultSprites[i];

                for (int j = 0; j < sprite.Height; j++)
                {
                    _ram[i * sprite.Height + j] = sprite.Rows[j];
                }
            }
        }
        
        public void ProceedCycle()
        {
            if (_isWaitingForKeyPress)
            {
                
            }
            
            // 1) Timers
            if (_registerDT > 0)
                _registerDT -= 1;

            if (_registerST > 0)
            {
                Beeper.Beep();
                _registerST -= 1;
            }
            
            //var instruction = ReadNext();
            //InstructionAsEnum(1);

            for (int i = 0; i < _loadedRomSize / 2; i++)
            {
                var instr = ReadNext();
                Console.WriteLine( Convert.ToString(instr, 16).PadLeft(4, '0').ToUpper());
                var instrEnum = InstructionAsEnum(instr);
                Console.WriteLine(instrEnum);
            }

            // Console.WriteLine(Convert.ToString(instruction, 16));
        }

        private Instructions InstructionAsEnum(ushort instruction)
        {
            const ushort mask = 0xF000;

            switch (instruction & mask)
            {
                case 0x0000:
                    if ((instruction & 0xFFFF) == 0x00E0)
                        return Instructions.I_00E0;
                    else if ((instruction & 0xFFFF) == 0x00EE)
                        return Instructions.I_00EE;
                    else
                        return Instructions.I_0NNN;
                
                case 0x1000:
                    return Instructions.I_1NNN;
                
                case 0x2000:
                    return Instructions.I_2NNN;
                
                case 0x3000:
                    return Instructions.I_3XNN;
                
                case 0x4000:
                    return Instructions.I_4XNN;

                case 0x5000:
                    if ((instruction & 0xF00F) == 0x5000)
                        return Instructions.I_5XY0;
                    break;
                
                case 0x6000:
                    return Instructions.I_6XNN;

                case 0x7000:
                    return Instructions.I_7XNN;

                case 0x8000:
                    if ((instruction & 0xF00F) == 0x8000)
                        return Instructions.I_8XY0;
                    else if ((instruction & 0xF00F) == 0x8001)
                        return Instructions.I_8XY1;
                    else if ((instruction & 0xF00F) == 0x8002)
                        return Instructions.I_8XY2;
                    else if ((instruction & 0xF00F) == 0x8003)
                        return Instructions.I_8XY3;
                    else if ((instruction & 0xF00F) == 0x8004)
                        return Instructions.I_8XY4;
                    else if ((instruction & 0xF00F) == 0x8005)
                        return Instructions.I_8XY5;
                    else if ((instruction & 0xF00F) == 0x8006)
                        return Instructions.I_8XY6;
                    else if ((instruction & 0xF00F) == 0x8007)
                        return Instructions.I_8XY7;
                    else
                        return Instructions.I_8XYE;
                
                case 0x9000:
                    if ((instruction & 0xF00F) == 0x9000)
                        return Instructions.I_9XY0;
                    break;

                case 0xA000:
                    return Instructions.I_ANNN;
                
                case 0xB000:
                    return Instructions.I_BNNN;
                
                case 0xC000:
                    return Instructions.I_CXNN;
                
                case 0xD000:
                    return Instructions.I_DXYN;
                
                case 0xE000:
                    if ((instruction & 0xF0FF) == 0xE09E)
                        return Instructions.I_EX9E;
                    else if ((instruction & 0x00FF) == 0xE0A1)
                        return Instructions.I_EXA1;
                    break;
                
                case 0xF000:
                    if ((instruction & 0xF0FF) == 0xF007)
                        return Instructions.I_FX07;
                    else if ((instruction & 0xF0FF) == 0xF00A)
                        return Instructions.I_FX0A;
                    else if ((instruction & 0xF0FF) == 0xF015)
                        return Instructions.I_FX15;
                    else if ((instruction & 0xF0FF) == 0xF018)
                        return Instructions.I_FX18;
                    else if ((instruction & 0xF0FF) == 0xF01E)
                        return Instructions.I_FX1E;
                    else if ((instruction & 0xF0FF) == 0xF029)
                        return Instructions.I_FX29;
                    else if ((instruction & 0xF0FF) == 0xF033)
                        return Instructions.I_FX33;
                    else if ((instruction & 0xF0FF) == 0xF055)
                        return Instructions.I_FX55;
                    else if ((instruction & 0xF0FF) == 0xF065)
                        return Instructions.I_FX65;
                    break;
            }
            
            throw new ArgumentOutOfRangeException(
                nameof(instruction), 
                $"Invalid instruction: {Convert.ToString(instruction, 16).ToUpper().PadLeft(4, '0')} at position {_registerPC}"
                );
        }

        private void ProcessInstruction(Instructions instructionEnum, ushort instruction)
        {
            switch (instructionEnum)
            {
                case Instructions.I_0NNN:
                    break;
                case Instructions.I_00E0:
                    Display.Clear();
                    break;
                case Instructions.I_00EE:
                    break;
                case Instructions.I_1NNN:
                    
                    break;
                case Instructions.I_2NNN:
                    break;
                case Instructions.I_3XNN:
                    
                    break;
                case Instructions.I_4XNN:
                    break;
                case Instructions.I_5XY0:
                    break;
                case Instructions.I_6XNN:
                    break;
                case Instructions.I_7XNN:
                    break;
                case Instructions.I_8XY0:
                    break;
                case Instructions.I_8XY1:
                    break;
                case Instructions.I_8XY2:
                    break;
                case Instructions.I_8XY3:
                    break;
                case Instructions.I_8XY4:
                    break;
                case Instructions.I_8XY5:
                    break;
                case Instructions.I_8XY6:
                    break;
                case Instructions.I_8XY7:
                    break;
                case Instructions.I_8XYE:
                    break;
                case Instructions.I_9XY0:
                    break;
                case Instructions.I_ANNN:
                    break;
                case Instructions.I_BNNN:
                    break;
                case Instructions.I_CXNN:
                    break;
                case Instructions.I_DXYN:
                    break;
                case Instructions.I_EX9E:
                    break;
                case Instructions.I_EXA1:
                    break;
                case Instructions.I_FX07:
                    break;
                case Instructions.I_FX0A:
                    break;
                case Instructions.I_FX15:
                    break;
                case Instructions.I_FX18:
                    break;
                case Instructions.I_FX1E:
                    break;
                case Instructions.I_FX29:
                    break;
                case Instructions.I_FX33:
                    break;
                case Instructions.I_FX55:
                    break;
                case Instructions.I_FX65:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instructionEnum), instructionEnum, null);
            }
        }

        private bool Test(ushort number, ushort mask, ushort expected)
        {
            return (number & mask) == expected;
        }
        
        private ushort ReadNext()
        {
            ushort resultInstruction = 0;
            resultInstruction |= _ram[LoadingAddress + _registerPC++];
            resultInstruction <<= 8;
            resultInstruction |= _ram[LoadingAddress + _registerPC++];

            return resultInstruction;
        }

        public void LoadRom(string filename)
        {
            var bufferSize = _ram.Length - LoadingAddress;
            var memoryOffset = LoadingAddress;
            int size;
            
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    var bytes = br.ReadBytes(bufferSize);
                    size = bytes.Length;
                    
                    foreach (var b in bytes)
                    {
                        _ram[memoryOffset++] = b;
                    }
                }
            } 
            
            _loadedRomSize = size;
            Console.WriteLine($"Loaded ROM with size of {size} bytes");
        }

        public void DumpProgramMemory()
        {
            for (int i = LoadingAddress; i < LoadingAddress + _loadedRomSize; i++)
            {
                if (i != LoadingAddress && i % 2 == 0) 
                    Console.Write(" ");
                if (i != LoadingAddress && i % 32 == 0)
                    Console.WriteLine();
                
                var str = Convert.ToString(_ram[i], 16).ToUpper().PadLeft(2, '0');
                
                Console.Write($"{str}");
            }

            Console.WriteLine();
        }
    }
}