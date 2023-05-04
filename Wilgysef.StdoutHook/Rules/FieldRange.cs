using System;
using System.Diagnostics.CodeAnalysis;

namespace Wilgysef.StdoutHook.Rules
{
    public class FieldRange
    {
        public int Min { get; set; }

        public int? Max { get; set; }

        public bool InfiniteMax => !Max.HasValue;

        public int? SingleValue => Max.HasValue && Max.Value == Min
            ? (int?)Min
            : null;

        public FieldRange(int number) : this(number, number) { }

        public FieldRange(int min, int? max)
        {
            if (min <= 0)
            {
                throw new ArgumentException("Minimum cannot be less than 1.");
            }

            if (max.HasValue)
            {
                if (max.Value < 0)
                {
                    throw new ArgumentException("Maximum cannot be negative.");
                }
                if (min > max.Value)
                {
                    throw new ArgumentException("Minimum cannot be greater than maximum", nameof(min));
                }
            }

            Min = min;
            Max = max;
        }

        public bool Contains(int number)
        {
            return Min <= number && (!Max.HasValue || number <= Max.Value);
        }

        public static FieldRange Parse(string s)
        {
            if (!TryParse(s, out var range))
            {
                throw new ArgumentException("Invalid field range", nameof(s));
            }

            return range;
        }

        public static bool TryParse(string s, [MaybeNullWhen(false)] out FieldRange range)
        {
            if (int.TryParse(s, out var number))
            {
                return CreateRange(number, number, out range);
            }

            var index = s.IndexOf('-');
            if (index == -1)
            {
                range = null;
                return false;
            }

            if (!int.TryParse(s.AsSpan(0, index), out var min))
            {
                range = null;
                return false;
            }

            var maxSpan = s.AsSpan(index + 1);
            int? max = null;

            if (!maxSpan.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(maxSpan, out var maxInt))
                {
                    range = null;
                    return false;
                }

                max = maxInt;
            }

            return CreateRange(min, max, out range);

            static bool CreateRange(int min, int? max, out FieldRange? range_)
            {
                try
                {
                    range_ = new FieldRange(min, max);
                    return true;
                }
                catch
                {
                    // TODO: avoid catching?
                    range_ = null;
                    return false;
                }
            }
        }
    }
}
