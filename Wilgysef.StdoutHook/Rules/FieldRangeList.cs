using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Wilgysef.StdoutHook.Rules
{
    public class FieldRangeList
    {
        public IList<FieldRange> Fields => _fields;

        private readonly List<FieldRange> _fields = new List<FieldRange>();

        public FieldRangeList(params FieldRange[] fields) : this((IEnumerable<FieldRange>)fields) { }

        public FieldRangeList(IEnumerable<FieldRange> fields)
        {
            _fields.AddRange(fields);
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
}
