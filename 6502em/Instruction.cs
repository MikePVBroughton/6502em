using System;
using System.Collections.Generic;
using System.Text;

namespace _6502em
{
    /// <summary>
    /// A class which holds all the information required for a single opcode
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// The opcode of the instruction
        /// </summary>
        public byte OpCode;

        /// <summary>
        /// The string format to display on the screen
        /// </summary>
        public string Format;

        /// <summary>
        /// The base number of cycles which the operation takes
        /// </summary>
        public byte Cycles;

        /// <summary>
        /// The maximum number of cycles this operand can take.  Take the largest 
        /// if more than one extension cycle.
        /// </summary>
        public byte ExtraCycles;

        /// <summary>
        /// The number of bytes required for theis opcode, minimum 1.
        /// </summary>
        public byte Bytes;
    }

    /// <summary>
    /// A helper class which has all the opcodes currently usable by this emulator
    /// </summary>
    public class InstructionSet
    {
        /// <summary>
        /// All instructions.
        /// </summary>
        public Dictionary<UInt16, Instruction> Instructions = new Dictionary<UInt16, Instruction>();

        public InstructionSet()
        {
            Add(0xa9, 2, 2, 0, "LDA #${0:x2}");
            Add(0xad, 3, 4, 0, "LDA ${1:x2}{0:x2}");    // Absolute
            Add(0xbd, 3, 4, 1, "LDA ${1:x2}{0:x2}, X"); // not fully done
            Add(0xa1, 2, 6, 0, "LDA (${0:x2}, X)");     // not fully done
            Add(0xb1, 2, 5, 1, "LDA (${0:x2}), Y");     // not fully done
            Add(0xA2, 2, 2, 0, "LDX #${0:x2}");
            Add(0xD0, 2, 2, 2, "BNE ${0:x2}");
            Add(0x4c, 3, 3, 0, "JMP ${1:x2}{0:x2}");    // Direct
            Add(0x6C, 3, 5, 0, "JMP (${1:x2}{0:x2})");  // Indirect
            Add(0x20, 3, 6, 0, "JSR ${1:x2}{0:x2}");
            Add(0x60, 1, 6, 0, "RTS");
            Add(0xEA, 1, 2, 0, "NOP");
            Add(0xAA, 1, 2, 0, "TAX");
            Add(0x8A, 1, 2, 0, "TXA");
            Add(0xE8, 1, 2, 0, "INX");
            Add(0xCA, 1, 2, 0, "DEX");
            Add(0xa8, 1, 2, 0, "TAY");
            Add(0x98, 1, 2, 0, "TYA");
            Add(0xc8, 1, 2, 0, "INY");
            Add(0x88, 1, 2, 0, "DEY");
            Add(0x18, 1, 2, 0, "CLC");
            Add(0x38, 1, 2, 0, "SEC");
            Add(0x58, 1, 2, 0, "CLI");
            Add(0x78, 1, 2, 0, "SEI");
            Add(0xb8, 1, 2, 0, "CLV");
            Add(0xd8, 1, 2, 0, "CLD");
            Add(0xf8, 1, 2, 0, "SED");
            Add(0x8d, 3, 4, 0, "STA ${1:x2}{0:x2}");    // Absolute
            Add(0xa0, 2, 2, 0, "LDY #${0:x2}");
            Add(0x8e, 3, 4, 0, "STX ${1:x2}{0:x2}");    // Absolute
            Add(0x6d, 3, 4, 0, "ADC ${1:x2}{0:x2}");    // TODO - set the carry flag somehow. Absolute

        }

        /// <summary>
        /// Helper to speed up adding instructions
        /// </summary>
        /// <param name="OpCode"></param>
        /// <param name="Bytes"></param>
        /// <param name="Cycles"></param>
        /// <param name="ExtraCycles"></param>
        /// <param name="Format"></param>
        private void Add(byte OpCode, byte Bytes, byte Cycles, byte ExtraCycles, string Format)
        {
            Instructions.Add(OpCode,
                new Instruction()
                {
                    OpCode = OpCode,
                    Cycles = Cycles,
                    ExtraCycles = ExtraCycles,
                    Bytes = Bytes,
                    Format = Format
                });
        }
    }
}
