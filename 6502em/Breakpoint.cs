using System;
using System.Collections.Generic;
using System.Text;

namespace _6502em
{
    /// <summary>
    /// The break point manager
    /// </summary>
    public class BreakpointMgr
    {
        /// <summary>
        /// All break point sets.  A set contains one or more break point conditions which
        /// must all pass for the breakpoint to trigger
        /// </summary>
        private readonly List<BreakPointSet> breakpointSets = new List<BreakPointSet>();

        /// <summary>
        /// Are we in STEP mode
        /// </summary>
        public bool StepMode = false;

        /// <summary>
        /// Returns a breakpointset if we should halt execution, otherwise null
        /// </summary>
        /// <param name="cpu"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public BreakPointSet ShallIBreak(CPU cpu, Cycle c)
        {
            foreach(BreakPointSet bs in breakpointSets)
            {
                if (bs.ShallIBreak(cpu, c))
                    return bs;
            }
            return null;
        }

        /// <summary>
        /// Adds the breakpointset to the manager
        /// </summary>
        /// <param name="bps"></param>
        public void AddBreakPointSet(BreakPointSet bps)
        {
            breakpointSets.Add(bps);
        }
    }

    /// <summary>
    /// A grouping of breakpoints
    /// </summary>
    public class BreakPointSet
    {
        /// <summary>
        /// Creates a breakpointset, with optional name and hit count
        /// </summary>
        /// <param name="count">The number of times this breakpoint should hit before halting execution</param>
        /// <param name="name">Optional name for the breakpointset</param>
        public BreakPointSet(int count, string name)
        {
            Count = count;
            if ( name != "")
                Name = name;
        }

        /// <summary>
        /// Creates a breakpointset, with optional name and hit count
        /// </summary>
        /// <param name="count">The number of times this breakpoint should hit before halting execution</param>
        public BreakPointSet(int count)
        {
            Count = count;
        }

        /// <summary>
        /// Creates a breakpointset, with optional name and hit count
        /// </summary>
        /// <param name="count">The number of times this breakpoint should hit before halting execution</param>
        public BreakPointSet()
        {
        }

        /// <summary>
        /// The name of the breakpointset
        /// </summary>
        private readonly string Name = "";

        private readonly List<Breakpoint> breakpoints = new List<Breakpoint>();

        /// <summary>
        /// The number of times to watch before breaking
        /// </summary>
        private int Count = 1;

        /// <summary>
        /// Tests all breakpoints within the breakpointset, and returns true if all pass AND
        /// the hit count is correct, otherwise false
        /// </summary>
        /// <param name="cpu"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool ShallIBreak(CPU cpu, Cycle c)
        {
            bool shallIBreak = true;

            foreach(Breakpoint bp in breakpoints)
            {
                switch (bp.Element)
                {
                    case "Opcode":
                        {
                            if (cpu.Opcode != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "PC":
                        {
                            if (c.StartPC != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "A":
                        {
                            if (cpu.A!= bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "X":
                        {
                            if (cpu.X != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "Y":
                        {
                            if (cpu.Y != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "SP":
                        {
                            if (cpu.SP != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "C":
                        {
                            if ((cpu.Flags&CPU.FlagCarry) != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "D":
                        {
                            if ((cpu.Flags & CPU.FlagDecimal) != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "V":
                        {
                            if ((cpu.Flags & CPU.FlagOverflow) != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "I":
                        {
                            if ((cpu.Flags & CPU.FlagInterruptDisable) != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "N":
                        {
                            if ((cpu.Flags & CPU.FlagNegative) != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                    case "Z":
                        {
                            if ((cpu.Flags & CPU.FlagZero) != bp.ValueForBreak)
                                shallIBreak = false;
                            break;
                        }
                }
            }

            if ( shallIBreak)
            {
                Count--;
                if (Count != 0)
                    shallIBreak = false;
            }

            return shallIBreak;
        }

        public override string ToString()
        {
            return "Break Point > " + Name;
        }

        public void AddBreakPoint(string element, int valueForBreak)
        {
            Breakpoint bp = new Breakpoint()
            {
                Element = element,
                ValueForBreak = valueForBreak
            };
            AddBreakPoint(bp);
        }

        public void AddBreakPoint(Breakpoint bp)
        {
            breakpoints.Add(bp);
        }
    }

    /// <summary>
    /// Instance of the actual breakpoint to test
    /// </summary>
    public class Breakpoint
    {
        /// <summary>
        /// The element to watch
        /// </summary>
        public string Element;

        /// <summary>
        /// The value to break on
        /// </summary>
        public int ValueForBreak;
    }
}
