using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoChip8
{
    public class VirtualMachine
    {
        public static ushort LoadingAddress => LoChip8.Ram.LoadingAddress;
        
        public IBeeper Beeper { get; }
        public Keypad Keypad { get; }
        public IDisplay Display { get; }
        public Ram Ram { get; }
        public Cpu Cpu { get; }
        public Cartridge Cartridge { get; }

        private bool _isInitialized = false;

        private List<Tuple<ushort, Instructions>> _prevInstructions = new List<Tuple<ushort, Instructions>>();
        
        public string RomFileName { get; }
        public int RomSize { get; private set; }

        public VirtualMachine(IBeeper beeper, Keypad keypad, IDisplay display, string romFileName)
        {
            Beeper = beeper;
            Keypad = keypad;
            Display = display;
            Ram = new Ram();
            Cpu = new Cpu(Ram, Display, Keypad);
            Cartridge = new Cartridge(Ram);
            RomFileName = romFileName;

            Keypad.KeyDown += KeypadOnKeyDown;
        }

        private void KeypadOnKeyDown(object sender, byte e)
        {
            if (Cpu.IsWaitingForKey)
            {
                Cpu.IsWaitingForKey = false;
                Cpu.Registers[Cpu.RegisterToStoreKey] = e;
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
                throw new InitializationException("Virtual Machine is already initialized");
            
            Ram.Reset();
            Cpu.Reset();
            Cpu.RegisterPC = Ram.LoadingAddress;

            RomSize = Cartridge.LoadRom(RomFileName);

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

            if (Cpu.RegisterDT > 0)
                Cpu.RegisterDT -= 1;

            if (Cpu.RegisterST > 0)
            {
                Beeper.Beep(Cpu.RegisterST);
                Cpu.RegisterST = 0;
            }
            
            // TODO: Check if halt of the ST and DT registers is needed
            if (Cpu.IsWaitingForKey)
            {
                return;
            }

            var instr = ReadNext();
            // _prevInstructions.Add(new Tuple<ushort, Instructions>(instr, instrEnum));
            Cpu.HandleInstruction(InstructionResolver.Resolve(instr));

            Cpu.Interrupt();
        }

        public ushort ReadNext()
        {
            ushort resultInstruction = (ushort) (Ram[Cpu.RegisterPC++] << 8);

            return (ushort) (resultInstruction | Ram[Cpu.RegisterPC++]);
        }
    }
}