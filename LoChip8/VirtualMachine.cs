namespace LoChip8
{
    public class VirtualMachine
    {
        public static short LoadingAddress => 0x200;
        public static short ReservedSpace  => 0x1FF;
        
        private byte[] _ram = new byte[4096]; // 4096 bytes (4KB)
        private byte[] _registers = new byte[16]; // General-purpose registers Vx (where x is from 0x0 to 0xF)
        private short _registerI;
        private short _registerPC; // Program Counter
        private byte _registerSP;  // Stack Pointer
        
        private short[] _stack = new short[16];

        private byte _registerDT; // Delay Timer
        private byte _registerST; // Sound Timer
    }
}