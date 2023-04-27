using Shouldly;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class FieldRangeTest
{
    [Fact]
    public void Single()
    {
        var range = new FieldRange(4);
        range.Min.ShouldBe(4);
        range.Max.ShouldBe(4);

        range = FieldRange.Parse("4");
        range.Min.ShouldBe(4);
        range.Max.ShouldBe(4);
    }

    [Fact]
    public void Range()
    {
        var range = new FieldRange(1, 3);
        range.Min.ShouldBe(1);
        range.Max.ShouldBe(3);

        range = FieldRange.Parse("1-3");
        range.Min.ShouldBe(1);
        range.Max.ShouldBe(3);
    }

    [Fact]
    public void Invalid()
    {
        Should.Throw<ArgumentException>(() => new FieldRange(3, 1));
        Should.Throw<ArgumentException>(() => new FieldRange(3, -1));
        Should.Throw<ArgumentException>(() => new FieldRange(-1));
        Should.Throw<ArgumentException>(() => FieldRange.Parse("abc"));
    }

    [Fact]
    public void Contains()
    {
        var range = new FieldRange(1, 3);
        range.Contains(0).ShouldBeFalse();
        range.Contains(1).ShouldBeTrue();
        range.Contains(2).ShouldBeTrue();
        range.Contains(3).ShouldBeTrue();
        range.Contains(4).ShouldBeFalse();
    }
}
