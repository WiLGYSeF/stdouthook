using System;
using System.Collections.Generic;
using System.Text;

namespace Wilgysef.StdoutHook.Profiles
{
    internal class ColorState
    {
        private const string ForegroundColorDefault = "39";
        private const string BackgroundColorDefault = "49";

        internal string ForegroundColor { get; private set; } = ForegroundColorDefault;

        internal string BackgroundColor { get; private set; } = BackgroundColorDefault;

        internal HashSet<int> Styles { get; } = new();

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
                            ForegroundColor = ForegroundColorDefault;
                            BackgroundColor = BackgroundColorDefault;
                            Styles.Clear();
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
                            Styles.Add(num);
                            break;
                        case 21:
                            Styles.Remove(1);
                            Styles.Add(21);
                            break;
                        case 25:
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                            Styles.Remove(num - 20);
                            break;
                        case 22:
                            Styles.Remove(1);
                            Styles.Remove(2);
                            break;
                        case 23:
                            Styles.Remove(3);
                            Styles.Remove(20);
                            break;
                        case 24:
                            Styles.Remove(4);
                            Styles.Remove(21);
                            break;
                        case 54:
                            Styles.Remove(51);
                            Styles.Remove(52);
                            break;
                        case 55:
                            Styles.Remove(53);
                            break;
                        case 65:
                            Styles.Remove(60);
                            Styles.Remove(61);
                            Styles.Remove(62);
                            Styles.Remove(63);
                            Styles.Remove(64);
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

        public ColorState Copy()
        {
            var copy = new ColorState
            {
                ForegroundColor = ForegroundColor,
                BackgroundColor = BackgroundColor,
            };

            foreach (var style in Styles)
            {
                copy.Styles.Add(style);
            }

            return copy;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder
                .Append("\x1b[")
                .Append(ForegroundColor)
                .Append(';')
                .Append(BackgroundColor);

            foreach (var style in Styles)
            {
                builder
                    .Append(';')
                    .Append(style);
            }

            return builder.Append('m')
                .ToString();
        }
    }
}
