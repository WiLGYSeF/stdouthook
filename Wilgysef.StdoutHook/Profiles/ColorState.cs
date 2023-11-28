using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Wilgysef.StdoutHook.Profiles;

internal class ColorState : IEquatable<ColorState>
{
    private HashSet<int> _styles = new();

    internal string? ForegroundColor { get; private set; }

    internal string? BackgroundColor { get; private set; }

    internal IReadOnlyCollection<int> Styles => _styles;

    public void UpdateState(ColorList colors, int position)
    {
        for (var colorIndex = 0; colorIndex < colors.Count; colorIndex++)
        {
            var colorEntry = colors[colorIndex];
            if (colorEntry.Position > position)
            {
                break;
            }

            var color = colorEntry.Color;
            if (color[^1] != 'm')
            {
                continue;
            }

            var end = color.Length - 1;

            for (var cur = 2; cur < end;)
            {
                if (!GetNextNum(color, cur, end, out var num, out var next))
                {
                    break;
                }

                // https://stackoverflow.com/a/33206814
                switch (num)
                {
                    case 0:
                        ForegroundColor = null;
                        BackgroundColor = null;
                        _styles.Clear();
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 20:
                    case 51:
                    case 52:
                    case 53:
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                        _styles.Add(num);
                        break;
                    case 21:
                        _styles.Remove(1);
                        _styles.Add(21);
                        break;
                    case 22:
                        _styles.Remove(1);
                        _styles.Remove(2);
                        break;
                    case 23:
                        _styles.Remove(3);
                        _styles.Remove(20);
                        break;
                    case 24:
                        _styles.Remove(4);
                        _styles.Remove(21);
                        break;
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                        _styles.Remove(num - 20);
                        break;
                    case 54:
                        _styles.Remove(51);
                        _styles.Remove(52);
                        break;
                    case 55:
                        _styles.Remove(53);
                        break;
                    case 65:
                        _styles.Remove(60);
                        _styles.Remove(61);
                        _styles.Remove(62);
                        _styles.Remove(63);
                        _styles.Remove(64);
                        break;
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 39:
                    case 90:
                    case 91:
                    case 92:
                    case 93:
                    case 94:
                    case 95:
                    case 96:
                    case 97:
                    case 99:
                        ForegroundColor = num.ToString();
                        break;
                    case 40:
                    case 41:
                    case 42:
                    case 43:
                    case 44:
                    case 45:
                    case 46:
                    case 47:
                    case 49:
                    case 100:
                    case 101:
                    case 102:
                    case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                        BackgroundColor = num.ToString();
                        break;
                    case 38:
                        {
                            var multiNum = GetNextMultiNum(color, next, end, out next);
                            if (multiNum != null)
                            {
                                ForegroundColor = "38;" + multiNum;
                            }
                        }

                        break;
                    case 48:
                        {
                            var multiNum = GetNextMultiNum(color, next, end, out next);
                            if (multiNum != null)
                            {
                                BackgroundColor = "48;" + multiNum;
                            }
                        }

                        break;
                    default:
                        break;
                }

                cur = next;
            }
        }

        static string? GetNextMultiNum(ReadOnlySpan<char> span, int start, int end, out int next)
        {
            if (!GetNextNum(span, start, end, out var nextNum, out next))
            {
                return null;
            }

            if (nextNum == 2)
            {
                GetNextEntryIndex(span, next, end, out next);
                GetNextEntryIndex(span, next, end, out next);
                var endBlue = GetNextEntryIndex(span, next, end, out next);
                return span[start..endBlue].ToString();
            }
            else if (nextNum == 5)
            {
                return span[start..GetNextEntryIndex(span, next, end, out next)].ToString();
            }

            return null;
        }

        static bool GetNextNum(ReadOnlySpan<char> span, int start, int end, out int num, out int next)
        {
            var entry = GetNextEntry(span, start, end, out next);
            if (entry.Length == 0)
            {
                num = 0;
                return true;
            }

            return int.TryParse(entry, out num);
        }

        static ReadOnlySpan<char> GetNextEntry(ReadOnlySpan<char> span, int start, int end, out int next)
        {
            return span[start..GetNextEntryIndex(span, start, end, out next)];
        }

        static int GetNextEntryIndex(ReadOnlySpan<char> span, int start, int end, out int next)
        {
            int index;
            var curEnd = end;

            for (index = start; index < end; index++)
            {
                if (span[index] == ';')
                {
                    curEnd = index;
                    index++;
                    break;
                }
            }

            next = index;
            return curEnd;
        }
    }

    public ColorState Diff(ColorState other)
    {
        var diff = new ColorState();

        if (ForegroundColor != other.ForegroundColor)
        {
            diff.ForegroundColor = other.ForegroundColor
                ?? "39";
        }

        if (BackgroundColor != other.BackgroundColor)
        {
            diff.BackgroundColor = other.BackgroundColor
                ?? "49";
        }

        foreach (var style in Styles)
        {
            if (!other._styles.Contains(style))
            {
                var inverseStyle = GetInverseStyle(style);
                if (inverseStyle.HasValue)
                {
                    diff._styles.Add(inverseStyle.Value);
                }
            }
        }

        foreach (var style in other._styles)
        {
            if (!_styles.Contains(style))
            {
                diff._styles.Add(style);
            }
        }

        return diff;
    }

    public ColorState Copy()
    {
        var copy = new ColorState
        {
            ForegroundColor = ForegroundColor,
            BackgroundColor = BackgroundColor,
        };

        foreach (var style in _styles)
        {
            copy._styles.Add(style);
        }

        return copy;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (ForegroundColor == null
            && BackgroundColor == null
            && _styles.Count == 0)
        {
            return "";
        }

        var builder = new StringBuilder();
        var hasColor = false;

        builder.Append("\x1b[");

        if (ForegroundColor != null)
        {
            builder.Append(ForegroundColor);
            hasColor = true;
        }

        if (BackgroundColor != null)
        {
            if (hasColor)
            {
                builder.Append(';');
            }

            builder.Append(BackgroundColor);
            hasColor = true;
        }

        foreach (var style in _styles)
        {
            if (hasColor)
            {
                builder.Append(';');
            }

            builder.Append(style);
            hasColor = true;
        }

        return builder.Append('m')
            .ToString();
    }

    public override bool Equals(object? obj)
        => obj is ColorState state && Equals(state);

    public bool Equals(ColorState? other)
    {
        if (other == null
            || ForegroundColor != other.ForegroundColor
            || BackgroundColor != other.BackgroundColor
            || _styles.Count != other._styles.Count)
        {
            return false;
        }

        return _styles.SetEquals(other._styles);
    }

    public int GetHashCode([DisallowNull] ColorState obj)
        => ToString().GetHashCode();

    private static int? GetInverseStyle(int style)
    {
        return style switch
        {
            1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 or 9 => style + 20,
            20 => 23,
            51 or 52 => 54,
            53 => 55,
            60 or 61 or 62 or 63 or 64 => 65,
            _ => null,
        };
    }
}
