using System;
using System.Diagnostics;

namespace Wilgysef.StdoutHook.Utilities
{
    internal class OffsetStopwatch : Stopwatch
    {
        public TimeSpan Offset { get; set; }

        public new TimeSpan Elapsed => base.Elapsed + Offset;

        public new long ElapsedMilliseconds => base.ElapsedMilliseconds + (long)Offset.TotalMilliseconds;

        public new long ElapsedTicks => base.ElapsedTicks + Offset.Ticks;
    }
}
