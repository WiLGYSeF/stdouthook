using Shouldly;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileTests;

public class FieldRangeListTest
{
    [Fact]
    public void Parse()
    {
        var ranges = FieldRangeList.Parse("1");
        ranges.Fields.Count.ShouldBe(1);
        ShouldFieldRangeBe(ranges.Fields[0], 1, 1);

        ranges = FieldRangeList.Parse("1-3");
        ranges.Fields.Count.ShouldBe(1);
        ShouldFieldRangeBe(ranges.Fields[0], 1, 3);

        ranges = FieldRangeList.Parse("1-3,4");
        ranges.Fields.Count.ShouldBe(2);
        ShouldFieldRangeBe(ranges.Fields[0], 1, 3);
        ShouldFieldRangeBe(ranges.Fields[1], 4, 4);

        ranges = FieldRangeList.Parse("1-3,4, 7-10");
        ranges.Fields.Count.ShouldBe(3);
        ShouldFieldRangeBe(ranges.Fields[0], 1, 3);
        ShouldFieldRangeBe(ranges.Fields[1], 4, 4);
        ShouldFieldRangeBe(ranges.Fields[2], 7, 10);

        Should.Throw<ArgumentException>(() => FieldRangeList.Parse("1-3,-4"));
    }

    private static void ShouldFieldRangeBe(FieldRange range, int min, int max)
    {
        range.Min.ShouldBe(min);
        range.Max.ShouldBe(max);
    }
}
