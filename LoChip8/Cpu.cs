using System;

namespace LoChip8
{
    public class Cpu
    {
        public byte[] Registers => _registers;

        public ushort RegisterPC
        {
            get => _registerPC;
            set => _registerPC = value;
        }
        public ushort RegisterI => _registerI;
        public byte RegisterSP => _registerSP;

        public byte RegisterDT
        {
            get => _registerDT;
            set => _registerDT = value;
        }

        public byte RegisterST
        {
            get => _registerST;
            set => _registerST = value;
        }
        public ushort[] Stack => _stack;
        
        public bool IsWaitingForKey { get; set; }
        public byte RegisterToStoreKey { get; private set; }

        public IDisplay Display { get; }
        
        private readonly byte[] _registers = new byte[16]; // General-purpose registers Vx (where x is from 0x0 to 0xF)
        private readonly ushort[] _stack = new ushort[16];
        private ushort _registerI;
        private ushort _registerPC; // Program Counter
        private byte _registerSP;  // Stack Pointer
        private byte _registerDT; // Delay Timer
        private byte _registerST; // Sound Timer
        
        private readonly Ram _ram;
        private readonly Random _random = new Random();
        private readonly Keypad _keypad;

        public Cpu(Ram ram, IDisplay display, Keypad keypad)
        {
            _ram = ram;
            Display = display;
            _keypad = keypad;
        }

        public void Reset()
        {
            for (var i = 0; i < _registers.Length; i++)
                _registers[i] = 0;
            for (var i = 0; i < _stack.Length; i++)
                _stack[i] = 0;

            _registerI = 0;
            _registerPC = Ram.LoadingAddress;
            _registerSP = 0;
            _registerDT = 0;
            _registerST = 0;
        }

        public void Interrupt()
        {
            
        }

        public void HandleInstruction(Instruction instruction)
        {
            var instructionEnum = instruction.InstructionEnum;
            
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
                // Jump to address NNN
                _registerPC = (ushort) instruction.Values[0];
            }
            else if (instructionEnum == Instructions.I_2NNN)
            {
                // Execute subroutine starting at address NNN
                _stack[++_registerSP] = _registerPC;
                _registerPC = (ushort) instruction.Values[0];
            }
            else if (instructionEnum == Instructions.I_3XNN)
            {
                // Skip the following instruction if the value of register VX equals NN
                if (_registers[instruction.Values[0]] == instruction.Values[1])
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_4XNN)
            {
                // Skip the following instruction if the value of register VX is not equal to NN
                if (_registers[instruction.Values[0]] != instruction.Values[1])
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_5XY0)
            {
                // Skip the following instruction if the value of register VX is equal to the value of register VY
                if (_registers[instruction.Values[0]] == _registers[instruction.Values[1]])
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_6XNN)
            {
                // Store number NN in register VX
                _registers[instruction.Values[0]] = (byte) instruction.Values[1];
            }
            else if (instructionEnum == Instructions.I_7XNN)
            {
                // Add the value NN to register VX. Overflow (carry) is ignored.
                unchecked
                {
                    _registers[instruction.Values[0]] += (byte) instruction.Values[1];    
                }
            }
            else if (instructionEnum == Instructions.I_8XY0)
            {
                // Store the value of register VY in register VX
                _registers[instruction.Values[0]] = _registers[instruction.Values[1]];
            }
            else if (instructionEnum == Instructions.I_8XY1)
            {
                // Set VX to VX OR VY
                _registers[instruction.Values[0]] = 
                    (byte) (_registers[instruction.Values[0]] | _registers[instruction.Values[1]]);
            }
            else if (instructionEnum == Instructions.I_8XY2)
            {
                // Set VX to VX AND VY
                _registers[instruction.Values[0]] = 
                    (byte) (_registers[instruction.Values[0]] & _registers[instruction.Values[1]]);
            }
            else if (instructionEnum == Instructions.I_8XY3)
            {
                // Set VX to VX XOR VY
                _registers[instruction.Values[0]] = 
                    (byte) (_registers[instruction.Values[0]] ^ _registers[instruction.Values[1]]);
            }
            else if (instructionEnum == Instructions.I_8XY4)
            {
                // Add the value of register VY to register VX
                // Set VF to 01 if a carry occurs
                // Set VF to 00 if a carry does not occur
                var regYValue = _registers[instruction.Values[1]];

                var regX = instruction.Values[0];
                var regXValue = _registers[regX];

                unchecked
                {
                    _registers[regX] += regYValue;
                }

                if (regXValue + regYValue > byte.MaxValue)
                {
                    _registers[0xF] = 0x01;
                }
                else
                {
                    _registers[0xF] = 0x00;
                }
            }
            else if (instructionEnum == Instructions.I_8XY5)
            {
                // Subtract the value of register VY from register VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                var regYValue = _registers[instruction.Values[1]];

                var regX = instruction.Values[0];
                var regXValue = _registers[regX];

                unchecked
                {
                    _registers[regX] -= regYValue;    
                }

                if (regXValue < regYValue)
                {
                    _registers[0xF] = 0x00;
                }
                else
                {
                    _registers[0xF] = 0x01;
                }
            }
            else if (instructionEnum == Instructions.I_8XY6)
            {
                // Store the value of register VY shifted right one bit in register VX
                // Set register VF to the least significant bit prior to the shift
                var yVal = _registers[instruction.Values[1]];
                var leastBit = yVal & 0b0000_0001;
                var yValShifted = yVal >> 1;

                _registers[instruction.Values[0]] = (byte) yValShifted;
                _registers[0xF] = (byte) leastBit;
            }
            else if (instructionEnum == Instructions.I_8XY7)
            {
                // Set register VX to the value of VY minus VX
                // Set VF to 00 if a borrow occurs
                // Set VF to 01 if a borrow does not occur
                var regYValue = _registers[instruction.Values[1]];
                
                var regX = instruction.Values[0];
                var regXValue = _registers[regX];

                unchecked
                {
                    _registers[regX] = (byte) (regYValue - regXValue);    
                }

                if (regYValue >= regXValue)
                {
                    _registers[0xF] = 0x01;
                }
                else
                {
                    _registers[0xF] = 0x00;
                }
            }
            else if (instructionEnum == Instructions.I_8XYE)
            {
                // Store the value of register VY shifted left one bit in register VX
                // Set register VF to the most significant bit prior to the shift
                var yVal = _registers[instruction.Values[1]];
                var mostBit = yVal & 0b1000_0000;
                var yValShifted = yVal << 1;

                _registers[instruction.Values[0]] = (byte) yValShifted;
                _registers[0xF] = (byte) mostBit;
            }
            else if (instructionEnum == Instructions.I_9XY0)
            {
                // Skip the following instruction if the value of register VX is not equal to the value of register VY
                if (_registers[instruction.Values[0]] != _registers[instruction.Values[1]])
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_ANNN)
            {
                // Store memory address NNN in register I
                _registerI = (ushort) instruction.Values[0];
            }
            else if (instructionEnum == Instructions.I_BNNN)
            {
                // Jump to address NNN + V0
                _registerPC = (ushort) (instruction.Values[0] + _registers[0x0]);
            }
            else if (instructionEnum == Instructions.I_CXNN)
            {
                // Set VX to a random number with a mask of NN
                var randomNum = _random.Next(byte.MinValue, byte.MaxValue);
                var mask = instruction.Values[1];

                _registers[instruction.Values[0]] = (byte) (randomNum & mask);
            }
            else if (instructionEnum == Instructions.I_DXYN)
            {
                // Draw a sprite at position VX, VY with N bytes of sprite data starting at the address stored in I
                // Set VF to 01 if any set pixels are changed to unset, and 00 otherwise
                var vx = _registers[instruction.Values[0]];
                var vy = _registers[instruction.Values[1]];
                var n = instruction.Values[2];
                
                byte[] bytes = new byte[n];
                
                for (ushort i = 0; i < n; i++)
                {
                    bytes[i] = _ram[(ushort)(_registerI + i)];
                }
                
                var sprite = new Sprite(bytes, (byte) n);
                _registers[0xF] = Display.DrawSprite(sprite, vx, vy);
            }
            else if (instructionEnum == Instructions.I_EX9E)
            {
                // Skip the following instruction if the key corresponding to the hex value currently stored in register VX is pressed
                // TODO: ADD INTERRUPTS
                if (_keypad.IsKeyDown(_registers[instruction.Values[0]]))
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_EXA1)
            {
                // Skip the following instruction if the key corresponding to the hex value currently stored in register VX is not pressed
                // TODO: ADD INTERRUPTS
                if (!_keypad.IsKeyDown(_registers[instruction.Values[0]]))
                {
                    _registerPC += 2;
                }
            }
            else if (instructionEnum == Instructions.I_FX07)
            {
                // Store the current value of the delay timer in register VX
                _registers[instruction.Values[0]] = _registerDT;
            }
            else if (instructionEnum == Instructions.I_FX0A)
            {
                // Wait for a keypress and store the result in register VX
                // TODO: Find a better way to handle the halt
                IsWaitingForKey = true;
                RegisterToStoreKey = (byte) instruction.Values[0];
            }
            else if (instructionEnum == Instructions.I_FX15)
            {
                // Set the delay timer to the value of register VX
                _registerDT = _registers[instruction.Values[0]];
            }
            else if (instructionEnum == Instructions.I_FX18)
            {
                // Set the sound timer to the value of register VX
                _registerST = _registers[instruction.Values[0]];
            }
            else if (instructionEnum == Instructions.I_FX1E)
            {
                // Add the value stored in register VX to register I
                unchecked
                {
                    _registerI += _registers[instruction.Values[0]];    
                }
            }
            else if (instructionEnum == Instructions.I_FX29)
            {
                var val = _registers[instruction.Values[0]];
                _registerI = _ram.GetSymbolAddress(val);
            }
            else if (instructionEnum == Instructions.I_FX33)
            {
                // Store the binary-coded decimal equivalent of the value stored in register VX at addresses I, I+1, and I+2
                byte val = _registers[instruction.Values[0]];
                var hundreds = val / 100;
                val %= 100;
                var tens = val / 10;
                val %= 10;

                _ram[_registerI] = (byte) hundreds;
                _ram[(ushort) (_registerI + 1)] = (byte) tens;
                _ram[(ushort) (_registerI + 2)] = (byte) val;
            }
            else if (instructionEnum == Instructions.I_FX55)
            {
                // Store the values of registers V0 to VX inclusive in memory starting at address I
                // I is set to I + X + 1 after operation
                var reg = instruction.Values[0];
                
                for (int i = 0; i <= reg; i++)
                {
                    _ram[(ushort) (_registerI + i)] = _registers[i];
                }

                _registerI += (ushort) (reg + 1);
            }
            else if (instructionEnum == Instructions.I_FX65)
            {
                // Fill registers V0 to VX inclusive with the values stored in memory starting at address I
                // I is set to I + X + 1 after operation
                var reg = instruction.Values[0];

                for (int i = 0; i <= reg; i++)
                {
                    _registers[i] = _ram[(ushort) (_registerI + i)];
                }
                
                _registerI += (ushort) (reg + 1);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(instructionEnum), instructionEnum, null);
            }
        }
    }
}