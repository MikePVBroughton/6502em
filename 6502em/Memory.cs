using System;
using System.Collections.Generic;
using System.Text;

namespace _6502em
{
    /// <summary>
    /// A representation of the memory for the 6502 cpu
    /// </summary>
    public class Memory
    {
        /// <summary>
        /// Max memory
        /// </summary>
        const UInt16 MAX_MEM = UInt16.MaxValue;

        /// <summary>
        /// The actual ram structure
        /// </summary>
        private readonly byte[] ram = null;

        public Memory()
        {
            ram = new byte[MAX_MEM];
        }

        /// <summary>
        /// Read a byte but do not alter the cycle counter
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ReadByteDebug(UInt16 Address)
        {
            return ram[Address];
        }

        /// <summary>
        /// Gets the byte at the address and updates the cycle count
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte GetByte(Cycle c, UInt16 Address)
        {
            return GetByte(c, Address, 0);
        }

        /// <summary>
        /// Gets the byte at the address plus offset and updates the cycle count
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte GetByte(Cycle c, UInt16 Address, Byte offset)
        {
            c.ActualCycles++;
            return ram[Address+offset];
        }

        /// <summary>
        /// Sets the byte at the address and offset and updates the cycle count
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Address"></param>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        public void SetByte(Cycle c, UInt16 Address, Byte offset, Byte value)
        {
            c.ActualCycles++;
            ram[Address + offset] = value;
        }

        /// <summary>
        /// Gets the word (little endian) at the address and updates the cycle count by 2
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public UInt16 GetWord(Cycle c, UInt16 Address)
        {
            return GetWord(c, Address, 0);
        }

        /// <summary>
        /// Gets the word (little endian) at the address plus offset and updates the cycle count by 2
        /// </summary>
        /// <param name="c"></param>
        /// <param name="Address"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public UInt16 GetWord(Cycle c, UInt16 Address, Byte offset)
        {
            //c.ActualCycles++;
            UInt16 address = GetByte(c, Address, offset); ;
            offset++;
            address |= (UInt16) (GetByte(c, Address, offset) << 8);
            return address;
        }

        /// <summary>
        /// A simple helper function to load a series of opcodes (programme) into memory
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="Bytes"></param>
        public void LoadMemnory (UInt16 Address, byte[] Bytes)
        {
            for (int i = 0; i< Bytes.Length; i++)
            {
                ram[Address + i] = Bytes[i];
            }
        }
    }
}
