using System;
using System.Collections.Generic;

namespace _6502em
{
    /// <summary>
    /// Represents a complete opcode operation, or cycle (not the right word 
    /// but it stuck with me).  This is not a 6502 feature, I use it for live debugging
    /// etc, and to track cycles and changes easily.
    /// </summary>
    public class Cycle
    {
        /// <summary>
        /// THe operand being acted on for this cycle
        /// </summary>
        public Instruction Operand;

        /// <summary>
        /// The actual number of cycles performed by this cycle.
        /// </summary>
        public byte ActualCycles = 1;

        /// <summary>
        /// The starting Program Counter
        /// </summary>
        public readonly UInt16 StartPC;

        /// <summary>
        /// A list of bytes after the opcode, for visual purposes only
        /// </summary>
        private readonly List<object> Bytes = new List<object>();

        public Cycle(UInt16 PC)
        {
            this.StartPC = PC;
        }

        /// <summary>
        /// Used for visual purposes only...
        /// </summary>
        /// <param name="RAM"></param>
        public void ReadOperand(Memory RAM)
        {
            UInt16 add = StartPC;
            add++;
            for (int i = 1; i < Operand.Bytes; i++)
            {
                Bytes.Add(RAM.ReadByteDebug(add++));
            }
        }

        /// <summary>
        /// Output the cycle information in a nice format. Visual only.
        /// </summary>
        /// <returns></returns>
        public string DebugRow()
        {
            // Byte firstly;
            String ret = "";

            foreach(byte b in Bytes)
                ret += String.Format("{0:x2} ", b);

            ret = ret.PadRight(10);

            ret += ": " + String.Format(Operand.Format, Bytes.ToArray());

            return ret;
        }
    }
}
