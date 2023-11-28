using Wilgysef.StdoutHook.Utilities;

namespace Wilgysef.StdoutHook.Tests.UtilityTests;

public class OffsetStopwatchTest
{
    [Fact]
    public async Task Offset()
    {
        var offset = TimeSpan.FromSeconds(2);
        var delay = TimeSpan.FromMilliseconds(250);

        var stopwatch = new OffsetStopwatch
        {
            Offset = offset,
        };

        stopwatch.Start();
        await Task.Delay(delay);
        stopwatch.Stop();

        stopwatch.Elapsed.ShouldBeInRange(offset, offset.Add(delay * 2));
        stopwatch.ElapsedMilliseconds.ShouldBeInRange((long)offset.TotalMilliseconds, (long)(offset.TotalMilliseconds + delay.TotalMilliseconds * 2));
        stopwatch.ElapsedTicks.ShouldBeInRange(offset.Ticks, offset.Ticks + delay.Ticks * 2);
    }
}
