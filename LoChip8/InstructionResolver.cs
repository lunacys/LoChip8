using System;

namespace LoChip8
{
    public static class InstructionResolver
    {
        public static Instruction Resolve(ushort instruction)
        {
            const ushort mask = 0xF000;

            switch (instruction & mask)
            {
                case 0x0000:
                    if ((instruction & 0xFFFF) == 0x00E0)
                        return new Instruction {InstructionEnum = Instructions.I_00E0};
                    else if ((instruction & 0xFFFF) == 0x00EE)
                        return new Instruction {InstructionEnum = Instructions.I_00EE};
                    else
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_0NNN,
                            Values = new [] { instruction & 0x0FFF }
                        };
                
                case 0x1000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_1NNN,
                        Values = new [] { instruction & 0x0FFF }
                    };
                
                case 0x2000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_2NNN,
                        Values = new [] { instruction & 0x0FFF }
                    };
                
                case 0x3000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_3XNN,
                        Values = new[]
                        {
                            (instruction >> 8) & 0x000F,
                            instruction & 0x00FF
                        }
                    };
                
                case 0x4000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_4XNN,
                        Values = new[]
                        {
                            (instruction >> 8) & 0x000F,
                            instruction & 0x00FF
                        }
                    };

                case 0x5000:
                    if ((instruction & 0xF00F) == 0x5000)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_5XY0,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    break;
                
                case 0x6000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_6XNN,
                        Values = new []
                        {
                            (instruction >> 8) & 0x000F,
                            instruction & 0x00FF
                        }
                    };

                case 0x7000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_7XNN,
                        Values = new []
                        {
                            (instruction >> 8) & 0x000F,
                            instruction & 0x00FF
                        }
                    };

                case 0x8000:
                    if ((instruction & 0xF00F) == 0x8000)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY0,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8001)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY1,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8002)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY2,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8003)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY3,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8004)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY4,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8005)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY5,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8006)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY6,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x8007)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XY7,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    else if ((instruction & 0xF00F) == 0x800E)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_8XYE,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    break;

                case 0x9000:
                    if ((instruction & 0xF00F) == 0x9000)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_9XY0,
                            Values = new []
                            {
                                (instruction >> 8) & 0x000F,
                                (instruction >> 4) & 0x000F
                            }
                        };
                    break;

                case 0xA000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_ANNN,
                        Values = new[] {instruction & 0x0FFF}
                    };
                
                case 0xB000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_BNNN,
                        Values = new[] {instruction & 0x0FFF}
                    };
                
                case 0xC000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_CXNN,
                        Values = new []
                        {
                            (instruction >> 8) & 0x000F,
                            instruction & 0x00FF
                        }
                    };
                
                case 0xD000:
                    return new Instruction
                    {
                        InstructionEnum = Instructions.I_DXYN,
                        Values = new []
                        {
                            (instruction >> 8) & 0x000F,
                            (instruction >> 4) & 0x000F,
                            (instruction) & 0x000F
                        }
                    };
                
                case 0xE000:
                    if ((instruction & 0xF0FF) == 0xE09E)
                    {
                        var key = (instruction >> 8) & 0x000F;
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_EX9E,
                            Values = new[] {key}
                        };
                    }
                    else if ((instruction & 0xF0FF) == 0xE0A1)
                    {
                        var key = (instruction >> 8) & 0x000F;
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_EXA1,
                            Values = new[] {key}
                        };
                    }
                    break;
                
                case 0xF000:
                    if ((instruction & 0xF0FF) == 0xF007)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX07,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF00A)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX0A,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF015)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX15,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF018)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX18,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF01E)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX1E,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF029)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX29,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF033)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX33,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF055)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX55,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    else if ((instruction & 0xF0FF) == 0xF065)
                        return new Instruction
                        {
                            InstructionEnum = Instructions.I_FX65,
                            Values = new[] {(instruction >> 8) & 0x000F}
                        };
                    break;
            }

            throw new ArgumentOutOfRangeException(
                nameof(instruction),
                $"Invalid instruction: {Convert.ToString(instruction, 16).ToUpper().PadLeft(4, '0')}"
            );
        }
    }
}