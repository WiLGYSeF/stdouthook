using Wilgysef.StdoutHook.Extensions;

namespace Wilgysef.StdoutHook.Tests.ExtensionTests;

public class IListExtensionsTest
{
    [Fact]
    public void InsertRange_Smaller_Beginning()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
        list.InsertRange(0, new List<int> { 10, 11 });

        list.SequenceEqual(new[] { 10, 11, 0, 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void InsertRange_Smaller_Middle()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
        list.InsertRange(2, new List<int> { 10, 11 });

        list.SequenceEqual(new[] { 0, 1, 10, 11, 2, 3, 4, 5 });
    }

    [Fact]
    public void InsertRange_Smaller_Penultimate()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
        list.InsertRange(list.Count - 1, new List<int> { 10, 11 });

        list.SequenceEqual(new[] { 0, 1, 2, 3, 4, 10, 11, 5 });
    }

    [Fact]
    public void InsertRange_Smaller_End()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3, 4, 5 };
        list.InsertRange(list.Count, new List<int> { 10, 11 });

        list.SequenceEqual(new[] { 0, 1, 2, 3, 4, 5, 10, 11 });
    }

    [Fact]
    public void InsertRange_Larger_Beginning()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3 };
        list.InsertRange(0, new List<int> { 10, 11, 12, 13 });

        list.SequenceEqual(new[] { 10, 11, 12, 13, 0, 1, 2, 3 });
    }

    [Fact]
    public void InsertRange_Larger_Middle()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3 };
        list.InsertRange(2, new List<int> { 10, 11, 12, 13 });

        list.SequenceEqual(new[] { 0, 1, 10, 11, 12, 13, 2, 3 });
    }

    [Fact]
    public void InsertRange_Larger_Penultimate()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3 };
        list.InsertRange(list.Count - 1, new List<int> { 10, 11, 12, 13 });

        list.SequenceEqual(new[] { 0, 1, 2, 10, 11, 12, 13, 3 });
    }

    [Fact]
    public void InsertRange_Larger_End()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3 };
        list.InsertRange(list.Count, new List<int> { 10, 11, 12, 13 });

        list.SequenceEqual(new[] { 0, 1, 2, 3, 10, 11, 12, 13 });
    }

    [Fact]
    public void InsertRange_Empty()
    {
        IList<int> list = new List<int>();
        list.InsertRange(0, new List<int> { 10, 11, 12, 13 });

        list.SequenceEqual(new[] { 10, 11, 12, 13 });
    }

    [Fact]
    public void InsertRange_Invalid()
    {
        IList<int> list = new List<int> { 0, 1, 2, 3 };
        Should.Throw<ArgumentOutOfRangeException>(() => list.InsertRange(10, new List<int> { 10, 11, 12, 13 }));
        Should.Throw<ArgumentOutOfRangeException>(() => list.InsertRange(-4, new List<int> { 10, 11, 12, 13 }));
    }
}
