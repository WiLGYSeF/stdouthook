using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Wilgysef.StdoutHook.Rules;

public class FieldRangeList
{
    private readonly List<FieldRange> _fields = new();

    public FieldRangeList(params FieldRange[] fields)
        : this((IEnumerable<FieldRange>)fields)
    {
    }

    public FieldRangeList(IEnumerable<FieldRange> fields)
    {
        _fields.AddRange(fields);
    }

    public IList<FieldRange> Fields => _fields;

    public int? SingleValue => Fields.Count == 1
        ? Fields[0].SingleValue
        : null;

    public int GetMin()
    {
        var min = int.MaxValue;

        for (var i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].Min < min)
            {
                min = _fields[i].Min;
            }
        }

        return min;
    }

    public int GetMax()
    {
        var max = 0;

        for (var i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].InfiniteMax)
            {
                return int.MaxValue;
            }

            if (_fields[i].Max!.Value > max)
            {
                max = _fields[i].Max!.Value;
            }
        }

        return max;
    }

    public bool IsInfiniteMax()
    {
        for (var i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].InfiniteMax)
            {
                return true;
            }
        }

        return false;
    }

    public bool Contains(int number)
    {
        for (var i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].Contains(number))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return string.Join(", ", Fields);
    }

    public static FieldRangeList Parse(string s)
    {
        if (!TryParse(s, out var ranges))
        {
            throw new ArgumentException("Invalid range list", nameof(s));
        }

        return ranges;
    }

    public static bool TryParse(string s, [MaybeNullWhen(false)] out FieldRangeList ranges)
    {
        ranges = new FieldRangeList();

        foreach (var range in s.Split(','))
        {
            if (!FieldRange.TryParse(range.Trim(), out var result))
            {
                ranges = null;
                return false;
            }

            ranges.Fields.Add(result);
        }

        return true;
    }
}
