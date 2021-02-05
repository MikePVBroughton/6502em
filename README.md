# 6502em

Just tried to write a 6502 emulator that can calculate Fibonacci sequence.

I also added a debugger that displays the code as it runs.  You can step execute, run to end of code or set as many breakpoints as you wish.

    PC   : BYTES        : Op                 A  X  Y  SP NV-bDIZC
    7009 : aa           : TAX                03 03 04 ff 00000000 2+0:2
    700a : 18           : CLC                03 03 04 ff 00000000 2+0:2
    700b : 6d 00 40     : ADC $4000          05 03 04 ff 00000000 4+0:4
    700e : 8e 00 40     : STX $4000          05 03 04 ff 00000000 4+0:4
    7011 : 88           : DEY                05 03 03 ff 00000000 2+0:2
    7012 : d0 f5        : BNE $f5            05 03 03 ff 00000000 2+2:3
    7009 : aa           : TAX                05 05 03 ff 00000000 2+0:2
    700a : 18           : CLC                05 05 03 ff 00000000 2+0:2
    700b : 6d 00 40     : ADC $4000          08 05 03 ff 00000000 4+0:4
    700e : 8e 00 40     : STX $4000          08 05 03 ff 00000000 4+0:4
    Break Point > 5th time at 0x700e
    PC   : BYTES        : Op                 A  X  Y  SP NV-bDIZC
    7011 : 88           : DEY                08 05 02 ff 00000000 2+0:2
    Break Point > A = 8 and Y=2 for the first time

*Example above is Fibonacci, A holding the sequence which shows 3,5 and 8 being computed)*

At a break point, if you press a cursor key it will step to the next instruction.  If you press return at any 
stage, the process will run to the end or to the next breakpoint.  I don't have all the operands defined, most simple
ones and the ones required for the fibonacci sequence.

There is a load function which requires a memory address and a byte array of what to load.

The last columns above are debug columns showing the cycles expected, the extra and the total number of cycles
taken for the instructions.

Enjoy
