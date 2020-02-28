using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoChip8
{
    public class VirtualMachine
    {
        public static ushort LoadingAddress => 0x200;
        
        public IBeeper Beeper { get; }
        public Keypad Keypad { get; }
        public IDisplay Display { get; }
        
        private byte[] _ram = new byte[4096]; // 4096 bytes (4KB)

        public byte[] Ram => _ram;
        
        private byte[] _registers = new byte[16]; // General-purpose registers Vx (where x is from 0x0 to 0xF)

        public byte[] Registers => _registers;
        
        private ushort _registerI;
        private ushort _registerPC; // Program Counter

        public ushort RegisterPC => _registerPC;
        
        private byte _registerSP;  // Stack Pointer
        
        private ushort[] _stack = new ushort[16];

        private byte _registerDT; // Delay Timer
        private byte _registerST; // Sound Timer

        private int _loadedRomSize;

        private bool _isWaitingForKeyPress = false;
        private byte _registerToStoreKey;
        
        private Random _random = new Random();

        private bool _isInitialized = false;

        public VirtualMachine(IBeeper beeper, Keypad keypad, IDisplay display)
        {
            Beeper = beeper;
            Keypad = keypad;
            Display = display;
            
            Keypad.KeySent += KeypadOnKeySent;
        }

        private void KeypadOnKeySent(object sender, byte key)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            if (_isInitialized)
                throw new InitializationException("Virtual Machine is already initialized");
            
            var defaultSprites = Sprite.GenerateDefaultSprites();

            // Cleanup
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

            // Store default sprites in RAM
            for (int i = 0; i < defaultSprites.Count; i++)
            {
                var sprite = defaultSprites[i];

                for (int j = 0; j < sprite.Height; j++)
                {
                    _ram[i * sprite.Height + j] = sprite.Rows[j];
                }
            }

            _isInitialized = true;
        }
        
        /// <summary>
        /// Proceeds a single cycle producing a new frame.
        /// When the frame changes, timers are updated and the next instruction is processed
        /// as well as keyboard input. 
        /// </summary>
        public void ProceedCycle()
        {
            if (!_isInitialized)
                throw new InitializationException("Virtual Machine must be initialized before use");
            
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

            /*for (int i = 0; i < _loadedRomSize / 2; i++)
            {
                var instr = ReadNext();
                Console.WriteLine( Convert.ToString(instr, 16).PadLeft(4, '0').ToUpper());
                var instrEnum = InstructionAsEnum(instr);
                Console.WriteLine(instrEnum);
            }*/

            var instr = ReadNext();
            var instrEnum = InstructionAsEnum(instr);
            ProcessInstruction(instrEnum, instr);

            // Console.WriteLine(Convert.ToString(instruction, 16));
        }

        public Instructions InstructionAsEnum(ushort instruction)
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

        public void ProcessInstruction(Instructions instructionEnum, ushort value)
        {
            if (instructionEnum == Instructions.I_0NNN)
            {
                // Execute machine language subroutine at address NNN
                throw new NotImplementedException("This instruction is highly considered deprecated");
            }
            else if (instructionEnum == Instructions.I_00E0)
            {
                Display.Clear();
            }
            else if (instructionEnum == Instructions.I_00EE)
            {
                // Return from a subroutine
                _registerPC = _stack[_registerSP--];
            }
            else if (instructionEnum == Instructions.I_1NNN)
            {
                var address = value & 0x0FFF;
                _registerPC = (byte) (address);
            }
            else if (instructionEnum == Instructions.I_2NNN)
            {
                // Execute subroutine starting at address NNN
                _registerPC = _stack[_registerSP++];
            }
            else if (instructionEnum == Instructions.I_3XNN)
            {
                // Skip the following instruction if the value of register VX equals NN
                var val = value & 0x00FF;
                var regX = (value >> 8) & 0x000F;

                if (_registers[regX] == val)
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_4XNN)
            {
                // Skip the following instruction if the value of register VX is not equal to NN
                var val = value & 0x00FF;
                var regX = (value >> 8) & 0x000F;

                if (_registers[regX] != val)
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_5XY0)
            {
                // Skip the following instruction if the value of register VX is equal to the value of register VY
                var regY = (value >> 4) & 0x000F;
                var regX = (value >> 8) & 0x000F;

                if (_registers[regX] == _registers[regY])
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_6XNN)
            {
                // Store number NN in register VX
                var nn = value & 0x00FF;
                var register = (value >> 8) & 0x000F;
                _registers[register] = (byte) nn;
            }
            else if (instructionEnum == Instructions.I_7XNN)
            {
                // Add the value NN to register VX. Overflow (carry) is ignored.
                var nn = value & 0x00FF;
                var register = (value >> 8) & 0x000F;
                unchecked
                {
                    _registers[register] += (byte) nn;    
                }
            }
            else if (instructionEnum == Instructions.I_8XY0)
            {
                // Store the value of register VY in register VX
                var rY = (value >> 4) & 0x000F;
                var rX = (value >> 8) & 0x000F;
                _registers[rX] = _registers[rY];
            }
            else if (instructionEnum == Instructions.I_8XY1)
            {
                // Set VX to VX OR VY
                var regX = (value >> 8) & 0x000F;
                _registers[regX] = (byte) (_registers[regX] | _registers[(value >> 4) & 0x000F]);
            }
            else if (instructionEnum == Instructions.I_8XY2)
            {
                // Set VX to VX AND VY
                var regX = (value >> 8) & 0x000F;
                _registers[regX] = (byte) (_registers[regX] & _registers[(value >> 4) & 0x000F]);
            }
            else if (instructionEnum == Instructions.I_8XY3)
            {
                // Set VX to VX XOR VY
                var regX = (value >> 8) & 0x000F;
                _registers[regX] = (byte) (_registers[regX] ^ _registers[(value >> 4) & 0x000F]);
            }
            else if (instructionEnum == Instructions.I_8XY4)
            {
                // Add the value of register VY to register VX
                // Set VF to 01 if a carry occurs
                // Set VF to 00 if a carry does not occur
                var regYValue = _registers[(value >> 4) & 0x000F];

                var regX = (value >> 8) & 0x000F;
                var regXValue = _registers[regX];
                
                if (regXValue + regYValue > byte.MaxValue)
                {
                    _registers[regX] = (byte) ((regXValue + regYValue) % 256);
                    _registers[0xF] = 0x01;
                }
                else
                {
                    _registers[regX] += regYValue;
                    _registers[0xF] = 0x00;
                }
            }
            else if (instructionEnum == Instructions.I_8XY5)
            {
                // Subtract the value of register VY from register VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                var regYValue = _registers[(value >> 4) & 0x000F];
                
                var regX = (value >> 8) & 0x000F;
                var regXValue = _registers[regX];

                if (regXValue - regYValue < byte.MinValue)
                {
                    _registers[regX] = (byte) ((regXValue - regYValue) % 256);
                    _registers[0xF] = 0x00;
                }
                else
                {
                    _registers[regX] -= regYValue;
                    _registers[0xF] = 0x01;
                }
            }
            else if (instructionEnum == Instructions.I_8XY6)
            {
                // Store the value of register VY shifted right one bit in register VX
                // Set register VF to the least significant bit prior to the shift
                var yVal = _registers[(value >> 4) & 0x000F];
                var leastBit = yVal & 0b0000_0001;
                var yValShifted = yVal >> 1;

                _registers[(value >> 8) & 0x000F] = (byte) yValShifted;
                _registers[0xF] = (byte) leastBit;
            }
            else if (instructionEnum == Instructions.I_8XY7)
            {
                // Set register VX to the value of VY minus VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                var regYValue = _registers[(value >> 4) & 0x000F];
                
                var regX = (value >> 8) & 0x000F;
                var regXValue = _registers[regX];

                if (regYValue - regXValue < byte.MinValue)
                {
                    _registers[regX] = (byte) ((regYValue - regXValue) % 256);
                    _registers[0xF] = 0x00;
                }
                else
                {
                    _registers[regX] = (byte) (regYValue - regXValue);
                    _registers[0xF] = 0x01;
                }
            }
            else if (instructionEnum == Instructions.I_8XYE)
            {
                // Store the value of register VY shifted left one bit in register VX
                // Set register VF to the most significant bit prior to the shift
                var yVal = _registers[(value >> 4) & 0x000F];
                var mostBit = yVal & 0b1000_0000;
                var yValShifted = yVal << 1;

                _registers[(value >> 8) & 0x000F] = (byte) yValShifted;
                _registers[0xF] = (byte) mostBit;
            }
            else if (instructionEnum == Instructions.I_9XY0)
            {
                // Skip the following instruction if the value of register VX is not equal to the value of register VY
                var regY = (value >> 4) & 0x000F;
                var regX = (value >> 8) & 0x000F;

                if (_registers[regX] != _registers[regY])
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_ANNN)
            {
                // Store memory address NNN in register I
                _registerI = (ushort) (value & 0x0FFF);
            }
            else if (instructionEnum == Instructions.I_BNNN)
            {
                var address = value & 0x0FFF;
                _registerPC = (byte) (address + _registers[0x0]);
            }
            else if (instructionEnum == Instructions.I_CXNN)
            {
                // Set VX to a random number with a mask of NN
                var randomNum = _random.Next(byte.MinValue, byte.MaxValue);
                var mask = value & 0x00FF;

                _registers[(value >> 8) & 0x000F] = (byte) (randomNum & mask);
            }
            else if (instructionEnum == Instructions.I_DXYN)
            {
                // Draw a sprite at position VX, VY with N bytes of sprite data starting at the address stored in I
                // Set VF to 01 if any set pixels are changed to unset, and 00 otherwise
                var vx = _registers[(value >> 8) & 0x000F];
                var vy = _registers[(value >> 4) & 0x000F];
                var n = value & 0x000F;
                
                byte[] bytes = new byte[n];
                
                for (int i = 0; i < n; i++)
                {
                    bytes[i] = _ram[_registerI + i];
                }
                
                var sprite = new Sprite(bytes, (byte) n);
                _registers[0xF] = (byte) (Display.DrawSprite(sprite, vx, vy) ? 1 : 0);
            }
            else if (instructionEnum == Instructions.I_EX9E)
            {
                // Skip the following instruction if the key corresponding to the hex value currently stored in register VX is pressed
                var key = _registers[(value >> 8) & 0x000F];
                if (Keypad.IsKeyDown(key))
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_EXA1)
            {
                // Skip the following instruction if the key corresponding to the hex value currently stored in register VX is not pressed
                var key = _registers[(value >> 8) & 0x000F];
                if (!Keypad.IsKeyDown(key))
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_FX07)
            {
                // Store the current value of the delay timer in register VX
                _registers[(value >> 8) & 0x000F] = _registerDT;
            }
            else if (instructionEnum == Instructions.I_FX0A)
            {
                // Wait for a keypress and store the result in register VX
                _isWaitingForKeyPress = true;
                _registerToStoreKey = (byte) ((value >> 8) & 0x000F);
            }
            else if (instructionEnum == Instructions.I_FX15)
            {
                // Set the delay timer to the value of register VX
                _registerDT = _registers[(value >> 8) & 0x000F];
            }
            else if (instructionEnum == Instructions.I_FX18)
            {
                // Set the sound timer to the value of register VX
                _registerST = _registers[(value >> 8) & 0x000F];
            }
            else if (instructionEnum == Instructions.I_FX1E)
            {
                // Add the value stored in register VX to register I
                _registerI += _registers[(value >> 8) & 0x000F];
            }
            else if (instructionEnum == Instructions.I_FX29)
            {
                var val = _registers[(value >> 8) & 0x000F];
                _registerI = GetSymbolAddress(val);
            }
            else if (instructionEnum == Instructions.I_FX33)
            {
                // Store the binary-coded decimal equivalent of the value stored in register VX at addresses I, I+1, and I+2
                byte val = _registers[(value >> 8) & 0x000F];
                var hundreds = val / 100;
                val %= 100;
                var tens = val / 10;
                val %= 10;

                _ram[_registerI] = (byte) hundreds;
                _ram[_registerI + 1] = (byte) tens;
                _ram[_registerI + 2] = (byte) val;
            }
            else if (instructionEnum == Instructions.I_FX55)
            {
                // Store the values of registers V0 to VX inclusive in memory starting at address I
                // I is set to I + X + 1 after operation
                var reg = (value >> 8) & 0x000F;
                
                for (int i = 0; i < reg; i++)
                {
                    _ram[_registerI + i] = _registers[i];
                }

                _registerI += (byte) (reg + 1);
            }
            else if (instructionEnum == Instructions.I_FX65)
            {
                // Fill registers V0 to VX inclusive with the values stored in memory starting at address I
                // I is set to I + X + 1 after operation
                var reg = (value >> 8) & 0x000F;

                for (int i = 0; i < reg; i++)
                {
                    _registers[i] = _ram[_registerI + i];
                }
                
                _registerI += (byte) (reg + 1);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(instructionEnum), instructionEnum, null);
            }
        }

        public ushort ReadNext()
        {
            ushort resultInstruction = (ushort) (_ram[LoadingAddress + _registerPC++] << 8);

            return (ushort) (resultInstruction | _ram[LoadingAddress + _registerPC++]);
        }

        public int LoadRom(string filename)
        {
            if (!_isInitialized)
                throw new InitializationException("Virtual Machine must be initialized before use");
            
            var bufferSize = _ram.Length - LoadingAddress;
            
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    var bytes = br.ReadBytes(bufferSize);
                    
                    return LoadRom(bytes);
                }
            }
        }

        public int LoadRom(byte[] romData)
        {
            var memoryOffset = LoadingAddress;
            _loadedRomSize = romData.Length;
            
            foreach (var b in romData)
            {
                _ram[memoryOffset++] = b;
            }

            return _loadedRomSize;
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

        public ushort GetSymbolAddress(byte symbol)
        {
            // As the sprite data is stored starting form 0x0000 and it has size of 5 bytes,
            // return symbol * 5 (0xF - 15, address is 15 * 5 = 75 or 0x4B)
            return (ushort) (symbol * 5);
        }
    }
}