using System.Collections.Generic;

namespace LoChip8
{
    public class Instruction
    {
        public static int MaxInstructions => 3;
        
        public Instructions InstructionEnum { get; set; }
        public int[] Values { get; set; }
    }
}