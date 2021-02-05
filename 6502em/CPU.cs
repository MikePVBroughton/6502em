using System;
using System.Text;

namespace _6502em
{
    public class CPU
    {
        /// <summary>
        /// The Program Counter
        /// </summary>
        public UInt16 PC = 0;

        /// <summary>
        /// CPU ram and rom (only ram here)
        /// </summary>
        public Memory RAM = new Memory();

        /// <summary>
        /// Simulated list of instructions
        /// </summary>
        private readonly InstructionSet opcodes = new InstructionSet();

        /// <summary>
        /// The Stack Pointer
        /// </summary>
        public byte SP;

        /// <summary>
        /// The A Register
        /// </summary>
        public byte A;

        /// <summary>
        /// The X Register
        /// </summary>
        public byte X;

        /// <summary>
        /// The Y Register
        /// </summary>
        public byte Y;

        /// <summary>
        /// CPU flags
        /// </summary>
        public byte Flags;

        /// <summary>
        /// The current opcode being operated on
        /// </summary>
        public byte Opcode;

        public const byte FlagCarry = 1 << 0;
        public const byte FlagZero = 1 << 1;
        public const byte FlagInterruptDisable = 1 << 2;
        public const byte FlagDecimal = 1 << 3;
        public const byte FlagBFlag = 1 << 4;
        public const byte FlagStack5Flag = 1 << 5;
        public const byte FlagOverflow = 1 << 6;
        public const byte FlagNegative = 1 << 7;

        /// <summary>
        /// Resets the CPU, then starts the execution process
        /// </summary>
        public void Reset(BreakpointMgr bpm)
        {
            // This will cause a hard reset
            // to run programme at $fffc/$fffd
            PC = 0xFFFC; 
            
            // Reset stack
            SP = 0xFF;
            
            // Force 6502 to use 0x4C as first opcode on swith on.  This is a
            // JMP $FFFC - a direct JMP.
            Opcode = 0x4C;
            PC--;
            debugHeader = 0;
            
            // Loop provided memory not toally shafted. In real life this doesn't happen,
            // the CPU blindly reads the byte and hopes it can do something with it.
            while (Opcode != 0)
            {
                Cycle c = new Cycle(PC);

                // Print the header, allows us to see the opcode and PC
                // when things go very wrong.
                DebugHeader(Opcode);

                // In reality, we could strip out the C below as that is used just for
                // visual purposes....
                ExecuteInstruction(c, Opcode);

                // Output all registers and flags
                DebugTail(c, bpm);

                Opcode = RAM.GetByte(c, PC);
            }
        }

        /// <summary>
        /// Execute the current opcode at the program counter address
        /// </summary>
        /// <param name="c"></param>
        /// <param name="opcode"></param>
        public void ExecuteInstruction(Cycle c, byte opcode)
        {
            c.Operand = opcodes.Instructions[opcode];

            // For niceness only - no real CPU code here.
            c.ReadOperand(RAM);

            switch (opcode)
            {
                case 0xa1:
                    {//Add(0xa1, 2, 6, 0, "LDA (${1:x2}, X)");
                        A = RAM.GetByte(c, PC, 1);
                        SetZeroAndNegativeFlag(A);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xa0:
                    {//Add(0xa0, 2, 2, 0, "LDY #${1:x2}");
                        Y = RAM.GetByte(c, PC, 1);
                        SetZeroAndNegativeFlag(Y);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xa9:
                    {//Add(0xa9, 2, 2, 0, "LDA #${1:x2}");
                        A = RAM.GetByte(c, PC, 1);
                        SetZeroAndNegativeFlag(A);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xbd:
                    {//Add(0xbd, 3, 4, 1, "LDA ${2:x2}{1:x2}, X");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        A = RAM.GetByte(c, address);
                        SetZeroAndNegativeFlag(A);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xb1:
                    {//Add(0xb1, 2, 5, 1, "LDA (${1:x2}), Y");
                        A = RAM.GetByte(c, PC, 1);
                        SetZeroAndNegativeFlag(A);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xa2:
                    {// Add(0xA2, 1, 2, 0, "LDX #${1:x2}");
                        X = RAM.GetByte(c, PC, 1);
                        SetZeroAndNegativeFlag(X);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xe8:
                    {// Add(0xE8, 1, 2, 0, "INX");
                        X++;
                        SetZeroAndNegativeFlag(X);
                        c.ActualCycles++;
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xca:
                    {// Add(0xCA, 1, 2, 0, "DEX");
                        X--;
                        SetZeroAndNegativeFlag(X);
                        c.ActualCycles++;
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xc8:
                    {// Add(0xc8, 1, 2, 0, "INY");
                        Y++;
                        SetZeroAndNegativeFlag(Y);
                        PC += c.Operand.Bytes;
                        c.ActualCycles++;
                        break;
                    }
                case 0x88:
                    {// Add(0x88, 1, 2, 0, "DEY");
                        Y--;
                        SetZeroAndNegativeFlag(Y);
                        c.ActualCycles++;
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x18:
                    {//Add(0x18, 1, 2, 0, "CLC");
                        c.ActualCycles++;
                        Flags &= byte.MaxValue ^ (FlagCarry);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x38:
                    {//Add(0x38, 1, 2, 0, "SEC");
                        c.ActualCycles++;
                        Flags |= FlagCarry;
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x58:
                    {//Add(0x58, 1, 2, 0, "CLI");
                        c.ActualCycles++;
                        Flags &= byte.MaxValue ^ (FlagInterruptDisable);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x78:
                    {//Add(0x78, 1, 2, 0, "SEI");
                        c.ActualCycles++;
                        Flags |= FlagInterruptDisable;
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xb8:
                    {//Add(0xb8, 1, 2, 0, "CLV");
                        c.ActualCycles++;
                        Flags &= byte.MaxValue ^ (FlagOverflow);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xd8:
                    {//Add(0xd8, 1, 2, 0, "CLD");
                        c.ActualCycles++;
                        Flags &= byte.MaxValue ^ (FlagDecimal);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xf8:
                    {//Add(0xf8, 1, 2, 0, "SED");
                        c.ActualCycles++;
                        Flags |= FlagDecimal;
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xd0:
                    {//Add(0xD0, 2, 2, 2, "BNE ${1:x2}");
                        if ((Flags &= FlagZero) == 0)
                        {
                            byte off = RAM.GetByte(c, PC, 1);
                            if ((off & FlagNegative) > 0)
                                c.ActualCycles++;
                            sbyte realOff = ConvertTwosComplementByteToSByte(off);
                            PC = (UInt16)(PC + realOff);
                        }
                        else
                        {
                            c.ActualCycles++;
                        }
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x4c:
                    {//Add(0x4c, 3, 3, 0, "JMP ${2:x2}{1:x2}");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        PC = address;
                        break;
                    }
                case 0x6c:
                    {//Add(0x6c, 3, 3, 0, "JMP (${2:x2}{1:x2})");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        address = RAM.GetWord(c, address, 0);
                        PC = address;
                        break;
                    }
                case 0x20:
                    {//Add(0x20, 3, 6, 0, "JSR ${2:x2}{1:x2}"); 
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        PC += c.Operand.Bytes;
                        UInt16 add = PC;
                        add--;
                        byte lsb = (byte)(add);
                        byte msb = (byte)(add >> 8);
                        c.ActualCycles++;
                        StackPush(c, lsb);
                        StackPush(c, msb);
                        PC = address;
                        break;
                    }
                case 0x60:
                    {//Add(0x60, 1, 6, 0, "RTS");
                        byte msb = StackPop(c);
                        byte lsb = StackPop(c);
                        UInt16 address = lsb;
                        address |= (UInt16)(msb << 8);
                        address++;
                        c.ActualCycles++;
                        PC = address;
                        break;
                    }
                case 0xEA:
                    {//Add(0xEA, 1, 2, 0, "NOP");
                        // Do nothing....
                        PC += c.Operand.Bytes;
                        c.ActualCycles++;
                        break;
                    }
                case 0xaa:
                    {//Add(0xAA, 1, 2, 0, "TAX");
                        PC += c.Operand.Bytes;
                        X = A;
                        c.ActualCycles++;
                        SetZeroAndNegativeFlag(X);
                        break;
                    }
                case 0x8a:
                    {//Add(0x8A, 1, 2, 0, "TXA");
                        PC += c.Operand.Bytes;
                        A = X;
                        c.ActualCycles++;
                        SetZeroAndNegativeFlag(A);
                        break;
                    }
                case 0xa8:
                    {//Add(0xA8, 1, 2, 0, "TAY");
                        PC += c.Operand.Bytes;
                        Y = A;
                        c.ActualCycles++;
                        SetZeroAndNegativeFlag(Y);
                        break;
                    }
                case 0x98:
                    {//Add(0x98, 1, 2, 0, "TYA");
                        PC += c.Operand.Bytes;
                        A = Y;
                        c.ActualCycles++;
                        SetZeroAndNegativeFlag(A);
                        break;
                    }
                case 0x8d:
                    {//Add(0x8d, 3, 4, 0, "STA ${2:x2}{1:x2}");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        RAM.SetByte(c, address, 0, A);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0xad:
                    {//Add(0xad, 3, 4, 0, "LDA ${2:x2}{1:x2}");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        A = RAM.GetByte(c, address);
                        SetZeroAndNegativeFlag(A);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x8e:
                    {//Add(0x8e, 3, 4, 0, "STX ${2:x2}{1:x2}");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        RAM.SetByte(c, address, 0, X);
                        PC += c.Operand.Bytes;
                        break;
                    }
                case 0x6d:
                    {//Add(0x6d, 3, 4, 0, "ADC ${2:x2}{1:x2}");
                        UInt16 address = RAM.GetWord(c, PC, 1);
                        byte value = RAM.GetByte(c, address);
                        byte result = (byte)(A + value);
                        
                        A = result;
                        if ((Flags & FlagCarry) > 0) A++;

                        // TODO - how to set the carry flag...

                        SetZeroAndNegativeFlag(A);
                        PC += c.Operand.Bytes;
                        break;
                    }

            }
        }

        // EVERYTHING BELOW this line are helpers and debug related.  The primary CPU
        // and all functionality is above.

        /// <summary>
        /// Update the ZERO and NEGATIVE flags based on the passed in value
        /// </summary>
        /// <param name="Value"></param>
        private void SetZeroAndNegativeFlag(byte Value)
        {
            if (Value == 0)
            {
                Flags |= FlagZero;
            }
            else
            {
                Flags &= byte.MaxValue ^ (FlagZero);
            }

            if ((Value & FlagNegative) > 0)
            {
                Flags |= FlagNegative;
            }
            else
            {
                Flags &= byte.MaxValue ^ (FlagNegative);
            }
        }

        /// <summary>
        /// Used for debugging
        /// </summary>
        private int debugHeader = 0;

        /// <summary>
        /// Convert a byte to its 2's complement if negative bit set, otherwise
        /// return the byte unchanged.
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private sbyte ConvertTwosComplementByteToSByte(byte rawValue)
        {
            // If a positive value, return it
            if ((rawValue & 0x80) == 0)
            {
                return (sbyte) rawValue;
            }

            // Otherwise perform the 2's complement math on the value
            return (sbyte)(byte)((~(rawValue - 0x01)) * -1);
        }

        /// <summary>
        /// Push a byte onto the stack, update the cycles and alter the stack pointer
        /// </summary>
        /// <param name="c"></param>
        /// <param name="value"></param>
        private void StackPush(Cycle c, byte value)
        {
            // Create stack address
            UInt16 address = SP;
            address |= (UInt16)(0x01) << 8;
            RAM.SetByte(c, address, 0, value);
            SP--;
        }

        /// <summary>
        /// Pop a byte off the stack, update the cycles and alter the stack pointer
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private byte StackPop(Cycle c)
        {
            // Create stack address
            c.ActualCycles++; // Do I need this??
            SP++;
            UInt16 address = SP;
            address |= (UInt16)(0x01) << 8;
            return RAM.GetByte(c, address, 0);
        }

        /// <summary>
        /// Output various header information to the console. Visual only.
        /// </summary>
        /// <param name="opcode"></param>
        private void DebugHeader(byte opcode)
        {
            if (debugHeader++ % 10 == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("PC   : BYTES        : Op                 A  X  Y  SP NV-bDIZC");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write (String.Format("{0:x4} : {1:x2} ", PC, opcode));
        }

        /// <summary>
        /// Output various information to the console. Visual only.
        /// </summary>
        /// <param name="c"></param>
        private void DebugTail(Cycle c, BreakpointMgr bpm)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(c.DebugRow().PadRight(30));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(String.Format(" {0:x2} {1:x2} {2:x2} {3:x2} ", A, X, Y, SP));

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(ToBinary(Flags));

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(" " + c.Operand.Cycles + "+" + c.Operand.ExtraCycles+":");
            
            if ( c.ActualCycles < c.Operand.Cycles || c.ActualCycles > c.Operand.Cycles + c.Operand.ExtraCycles )
                Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(c.ActualCycles);

            BreakPointSet bps = bpm.ShallIBreak(this, c);
            if (bps != null || bpm.StepMode)
            {
                if (bps != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(bps);
                }
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                    bpm.StepMode = false;
                else
                    bpm.StepMode = true;
            }
        }

        /// <summary>
        /// Turn an byte into a binary string of 8 padding. Visual only
        /// </summary>
        /// <param name="myValue"></param>
        /// <returns></returns>
        private string ToBinary(int myValue)
        {
            string binVal = Convert.ToString(myValue, 2);
            int bits = 0;
            int bitblock = 8;

            for (int i = 0; i < binVal.Length; i += bitblock)
            { 
                bits += bitblock; 
            }

            return binVal.PadLeft(bits, '0');
        }
    }
}
