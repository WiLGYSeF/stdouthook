using System;
using System.Diagnostics.CodeAnalysis;

namespace Wilgysef.StdoutHook.Profiles
{
    public class FieldRange
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public FieldRange(int number) : this(number, number) { }

        public FieldRange(int min, int max)
        {
            if (min < 0)
            {
                throw new ArgumentException("Minimum cannot be negative.");
            }
            if (max < 0)
            {
                throw new ArgumentException("Maximum cannot be negative.");
            }
            if (min > max)
            {
                throw new ArgumentException("Minimum cannot be greater than maximum", nameof(min));
            }

            Min = min;
            Max = max;
        }

        public bool Contains(int number)
        {
            return Min <= number && number <= Max;
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
            if (index == -1
                || !int.TryParse(s[..index], out var min)
                || !int.TryParse(s[(index + 1)..], out var max))
            {
                range = null;
                return false;
            }

            return CreateRange(min, max, out range);

            static bool CreateRange(int min, int max, out FieldRange? range_)
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
