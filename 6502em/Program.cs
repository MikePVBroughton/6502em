using System;

namespace _6502em
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("6502 Emulator\n");

            CPU cpu = new CPU();

            // Emulate the 6502 start up, which looks for a vector at $FFFC/$FFFD
            // and starts execution from there. Need to start one code before as
            // we musn't have zero at the load point - any value other than 0 good.
            // This is not required if we don't mind blue screen of death situations
            cpu.RAM.LoadMemnory(0xFFFB, new byte[] { 0xff, 0x00, 0x50});

            // Variables for testing - including JMP($) for subroutine
            cpu.RAM.LoadMemnory(0x0302, new byte[] { 0xaa, 0xbb, 23, 0x50});
            
            // JSR and RET Subroutine Test
            cpu.RAM.LoadMemnory(0x6040, new byte[] { 0xa2, 0x05, 0x8a, 0xaa, 0xc8, 0x98,0xc8, 0xa8, 0x88, 0xe8, 0xca, 0x60 });
            
            // Main code. Tests nearly all functions, then finally JMPs to Fibonacci code
            cpu.RAM.LoadMemnory(0x5000, new byte[] { 0x6c, 0x04, 0x03, 0xa1, 0xed, 0xa1, 0x00, 0xa1, 0xed, 0xa9, 0x01, 0xbd, 0x02, 0x01, 0xb1, 0xff, 0xa2, 127, 
                    0xe8, 0xa2, 0xff, 0xe8, 0xca, 0xd0, 0x01, 0xea, 0xa2, 0x04, 0xca, 0xd0, 0xfd, 0xca, 0x20, 0x40, 0x60, 0xEA, 0x38, 0x18,
                    0xf8,0xd8,0x78,0x58, 0x8d, 0x00,0x40,0xa9,0x0,0xad,0x00,0x40, 0x8e,0x00,0x40 ,0xad,0x00,0x40,
                    0x4c, 0x00, 0x70});

            // Fibonacci Sequence
            cpu.RAM.LoadMemnory(0x7000, new byte[] { 0xa0, 0x07, 0xa9, 0x00, 0x8d, 0x00, 0x40, 0xa9, 0x01,
               0xaa,0x18,0x6d, 0x00, 0x40,0x8e, 0x00, 0x40,0x88,0xd0, 0xf5,0xea});

            // Add some breakpoints for fun.
            BreakpointMgr mgr = new BreakpointMgr();
            BreakPointSet bps = new BreakPointSet(5, "5th time at 0x700e");
            mgr.AddBreakPointSet(bps);
            bps.AddBreakPoint("PC", 0x700e);

            BreakPointSet bps2 = new BreakPointSet(1, "A = 8 and Y=2 for the first time");
            mgr.AddBreakPointSet(bps2);
            bps2.AddBreakPoint("A", 0x08);
            bps2.AddBreakPoint("Y", 0x02);

            // You dont need to name the break point, and if you dont provide a count
            // it will assume the first time the breakpoint is hit.
            BreakPointSet bps3 = new BreakPointSet();
            mgr.AddBreakPointSet(bps3);
            bps3.AddBreakPoint("Opcode", 0x18);

            BreakPointSet bps4 = new BreakPointSet(1, "Stack has exactly one entry on");
            mgr.AddBreakPointSet(bps4);
            bps4.AddBreakPoint("SP", 0xfd);

            BreakPointSet bps5 = new BreakPointSet(1, "Carry flag set");
            mgr.AddBreakPointSet(bps5);
            bps5.AddBreakPoint("C", 0x1);

            // Simulate a reset and start the execution
            cpu.Reset(mgr);
        }
    }
}
/*
 * The Fibonacci sequence programme used above.
 *  
 *   LDY #7    ; Load required number    0xa0, 0x07,
 *   LDA #0    ; Clear last value        0xa9, 0x00,
 *   STA LAST                            0x8d, 0x00, 0x40,
 *   LDA #1    ; Set initial increment   0xa9, 0x01,
 *  Loop:                               9pos
 *   TAX       ; Save current value      0xaa,
 *   CLC       ; Compute the next one    0x18,
 *   ADC LAST                            0x6d, 0x00, 0x40,
 *   STX LAST  ; Save new last value     0x8e, 0x00, 0x40,
 *   DEY                                 0x88,
 *   BNE Loop                            0xd0, 0xec,
 *   NOP       ; Yth value is now in A   0xea
 *   
*/