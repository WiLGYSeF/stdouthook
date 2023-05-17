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
        range.SingleValue.ShouldBe(4);

        range = FieldRange.Parse("4");
        range.Min.ShouldBe(4);
        range.Max.ShouldBe(4);
        range.SingleValue.ShouldBe(4);

        range = new FieldRange(1, 3);
        range.SingleValue.ShouldBeNull();
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
    public void Range_InfiniteMax()
    {
        var range = new FieldRange(1, null);
        range.Min.ShouldBe(1);
        range.Max.ShouldBeNull();

        range = FieldRange.Parse("1-*");
        range.Min.ShouldBe(1);
        range.Max.ShouldBeNull();
    }

    [Fact]
    public void Invalid()
    {
        Should.Throw<ArgumentException>(() => new FieldRange(3, 1));
        Should.Throw<ArgumentException>(() => new FieldRange(3, -1));
        Should.Throw<ArgumentException>(() => new FieldRange(-1));
        Should.Throw<ArgumentException>(() => FieldRange.Parse("abc"));
        Should.Throw<ArgumentException>(() => FieldRange.Parse("abc-def"));
        Should.Throw<ArgumentException>(() => FieldRange.Parse("1-eee"));
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

    [Fact]
    public void Contains_InfiniteMax()
    {
        var range = new FieldRange(1, null);
        range.Contains(0).ShouldBeFalse();
        range.Contains(1).ShouldBeTrue();
        range.Contains(2).ShouldBeTrue();
        range.Contains(3).ShouldBeTrue();
    }

    [Fact]
    public void Range_ToString()
    {
        new FieldRange(1, 4).ToString().ShouldBe("1-4");
        new FieldRange(5, null).ToString().ShouldBe("5-Inf");
        new FieldRange(7, 7).ToString().ShouldBe("7");
    }
}
