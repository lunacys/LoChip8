using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace LoChip8.Tests
{
    [TestFixture]
    public class VirtualMachineTest
    {
        private VirtualMachine _vm;
        private ConsoleBeeper _beeper;
        private Keypad _keypad;
        private Display _display;
        
        // BREAKOUT
        private static byte[] TestRomData => new byte[]
        {
            0x6E, 0x05, 0x65, 0x00, 0x6B, 0x06, 0x6A, 0x00, 0xA3, 0x0C, 0xDA, 0xB1, 0x7A, 0x04, 0x3A, 0x40, 
            0x12, 0x08, 0x7B, 0x02, 0x3B, 0x12, 0x12, 0x06, 0x6C, 0x20, 0x6D, 0x1F, 0xA3, 0x10, 0xDC, 0xD1, 
            0x22, 0xF6, 0x60, 0x00, 0x61, 0x00, 0xA3, 0x12, 0xD0, 0x11, 0x70, 0x08, 0xA3, 0x0E, 0xD0, 0x11, 
            0x60, 0x40, 0xF0, 0x15, 0xF0, 0x07, 0x30, 0x00, 0x12, 0x34, 0xC6, 0x0F, 0x67, 0x1E, 0x68, 0x01, 
            0x69, 0xFF, 0xA3, 0x0E, 0xD6, 0x71, 0xA3, 0x10, 0xDC, 0xD1, 0x60, 0x04, 0xE0, 0xA1, 0x7C, 0xFE, 
            0x60, 0x06, 0xE0, 0xA1, 0x7C, 0x02, 0x60, 0x3F, 0x8C, 0x02, 0xDC, 0xD1, 0xA3, 0x0E, 0xD6, 0x71, 
            0x86, 0x84, 0x87, 0x94, 0x60, 0x3F, 0x86, 0x02, 0x61, 0x1F, 0x87, 0x12, 0x47, 0x1F, 0x12, 0xAC, 
            0x46, 0x00, 0x68, 0x01, 0x46, 0x3F, 0x68, 0xFF, 0x47, 0x00, 0x69, 0x01, 0xD6, 0x71, 0x3F, 0x01, 
            0x12, 0xAA, 0x47, 0x1F, 0x12, 0xAA, 0x60, 0x05, 0x80, 0x75, 0x3F, 0x00, 0x12, 0xAA, 0x60, 0x01, 
            0xF0, 0x18, 0x80, 0x60, 0x61, 0xFC, 0x80, 0x12, 0xA3, 0x0C, 0xD0, 0x71, 0x60, 0xFE, 0x89, 0x03, 
            0x22, 0xF6, 0x75, 0x01, 0x22, 0xF6, 0x45, 0x60, 0x12, 0xDE, 0x12, 0x46, 0x69, 0xFF, 0x80, 0x60, 
            0x80, 0xC5, 0x3F, 0x01, 0x12, 0xCA, 0x61, 0x02, 0x80, 0x15, 0x3F, 0x01, 0x12, 0xE0, 0x80, 0x15, 
            0x3F, 0x01, 0x12, 0xEE, 0x80, 0x15, 0x3F, 0x01, 0x12, 0xE8, 0x60, 0x20, 0xF0, 0x18, 0xA3, 0x0E, 
            0x7E, 0xFF, 0x80, 0xE0, 0x80, 0x04, 0x61, 0x00, 0xD0, 0x11, 0x3E, 0x00, 0x12, 0x30, 0x12, 0xDE, 
            0x78, 0xFF, 0x48, 0xFE, 0x68, 0xFF, 0x12, 0xEE, 0x78, 0x01, 0x48, 0x02, 0x68, 0x01, 0x60, 0x04, 
            0xF0, 0x18, 0x69, 0xFF, 0x12, 0x70, 0xA3, 0x14, 0xF5, 0x33, 0xF2, 0x65, 0xF1, 0x29, 0x63, 0x37,
            0x64, 0x00, 0xD3, 0x45, 0x73, 0x05, 0xF2, 0x29, 0xD3, 0x45, 0x00, 0xEE, 0xF0, 0x00, 0x80, 0x00,
            0xFC, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private static ushort[] ValidInstructions => new ushort[]
        {
            0x6E05
        };
        
        private static ushort[] InvalidInstructions => new ushort[]
        {
            0xFA00, 0xF111, 0x8AAF
        };
        
        [SetUp]
        public void SetUp()
        {
            _beeper = new ConsoleBeeper();
            _keypad = new Keypad();
            _display = new Display();
            
            _vm = new VirtualMachine(_beeper, _keypad, _display);
            
            using (var fs = new FileStream("BREAKOUT.ch8", FileMode.Create, FileAccess.Write))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(TestRomData);
                }
            }
        }

        [Test]
        public void Initialization_Test()
        {
            Assert.Throws<InitializationException>(() => { _vm.LoadRom("TESTROM.ch8"); });
            Assert.Throws<InitializationException>(() => { _vm.ProceedCycle(); });
            Assert.DoesNotThrow(() => { _vm.DumpProgramMemory(); });
            
            Assert.DoesNotThrow(() => { _vm.Initialize(); } );

            Assert.Throws<InitializationException>(() => { _vm.Initialize(); });
            
            Assert.IsInstanceOf<IBeeper>(_beeper);
            Assert.IsInstanceOf<IDisplay>(_display);
        }

        [Test]
        public void DumpProgramMemory_Test()
        {
            Assert.DoesNotThrow(() => { _vm.DumpProgramMemory(); });
        }
        
        [Test]
        public void RamLoad_Test()
        {
            _vm.Initialize();

            int romSize = 0;
            
            Assert.DoesNotThrow(() => { romSize = _vm.LoadRom("BREAKOUT.ch8"); });
            
            Assert.That(romSize > 0, "ROM Size must be higher that 1. ROM was loaded incorrectly.");
            Assert.That(romSize == TestRomData.Length, "ROM Size must be the same size as the byte array. ROM was loaded incorrectly.");
            
            for (int i = 0; i < romSize; i++)
            {
                Assert.AreEqual(_vm.Ram[VirtualMachine.LoadingAddress + i], TestRomData[i]);
            }
        }

        [Test]
        public void RamInterpretation_Test()
        {
            _vm.Initialize();
            
            var romSize = _vm.LoadRom("BREAKOUT.ch8");
            
            var instructions = new List<ushort>();
            
            for (int i = 0; i < romSize / 2; i++)
            {
                var instr = _vm.ReadNext();
                instructions.Add(instr);
            }

            Assert.That(instructions.Count == romSize / 2);

            // Must throw as there is incorrect instructions in the ROM that are actually sprite data
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                foreach (var instruction in instructions)
                {
                    _vm.InstructionAsEnum(instruction);
                }
            });
        }

        [Test]
        public void Interpretation_Test()
        {
            byte[] testRom = {
                0x6A, 0x10, // Store number 0x10 in register A
                0x7B, 0xFF, // Add the value NN to register VX
                0x6C, 0x1F,
                0x7C, 0xFF, // Here we check if in case of overflow everything works normally
                0x10, 0x00, // Jump to the beginning of the program
            };
            
            _vm.Initialize();
            _vm.LoadRom(testRom);

            var instruction = _vm.ReadNext();
            Assert.AreEqual(ConvertBytesToInstruction(testRom[0], testRom[1]), instruction);
            var instrAsEnum = _vm.InstructionAsEnum(instruction);
            Assert.AreEqual(Instructions.I_6XNN, instrAsEnum);

            _vm.ProcessInstruction(instrAsEnum, instruction);
            
            Assert.AreEqual(0x10, _vm.Registers[0xA]);
            
            var instruction2 = _vm.ReadNext();
            Assert.AreEqual(ConvertBytesToInstruction(testRom[2], testRom[3]), instruction2);
            var instrAsEnum2 = _vm.InstructionAsEnum(instruction2);
            Assert.AreEqual(Instructions.I_7XNN, instrAsEnum2);
            
            _vm.ProcessInstruction(instrAsEnum2, instruction2);

            Assert.AreEqual(0xFF, _vm.Registers[0xB]);
            
            // Check overflow
            var i3 = _vm.ReadNext();
            var ie3 = _vm.InstructionAsEnum(i3);
            _vm.ProcessInstruction(ie3, i3);

            var i4 = _vm.ReadNext();
            var ie4 = _vm.InstructionAsEnum(i4);
            Assert.DoesNotThrow(() => { _vm.ProcessInstruction(ie4, i4); });
            
            Assert.AreEqual(0x1E, _vm.Registers[0xC]);
            
            // Test jump
            var i5 = _vm.ReadNext();
            var i5e = _vm.InstructionAsEnum(i5);
            Assert.AreEqual(Instructions.I_1NNN, i5e);
            _vm.ProcessInstruction(i5e, i5);
            Assert.AreEqual(0x0000, _vm.RegisterPC);
            
            // Test the first two instructions
            instruction = _vm.ReadNext();
            Assert.AreEqual(ConvertBytesToInstruction(testRom[0], testRom[1]), instruction);
            instrAsEnum = _vm.InstructionAsEnum(instruction);
            Assert.AreEqual(Instructions.I_6XNN, instrAsEnum);

            _vm.ProcessInstruction(instrAsEnum, instruction);
            
            Assert.AreEqual(0x10, _vm.Registers[0xA]);
            
            instruction2 = _vm.ReadNext();
            Assert.AreEqual(ConvertBytesToInstruction(testRom[2], testRom[3]), instruction2);
            instrAsEnum2 = _vm.InstructionAsEnum(instruction2);
            Assert.AreEqual(Instructions.I_7XNN, instrAsEnum2);
            
            _vm.ProcessInstruction(instrAsEnum2, instruction2); // Here goes an overflow (0xFF + 0xFF)
            
            Assert.AreEqual(0xFE, _vm.Registers[0xB]);
        }

        private int ConvertBytesToInstruction(byte b1, byte b2)
        {
            return (b1 << 8) | b2;
        }
    }
}