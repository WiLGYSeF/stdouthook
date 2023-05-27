using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Extensions;

namespace Wilgysef.StdoutHook.Formatters
{
    internal static class ColorExtractor
    {
        private static readonly Regex ColorRegex = new Regex(@"\x1b\[([0-9;]*)m", RegexOptions.Compiled);

        public static string ExtractColor(string data, IList<KeyValuePair<int, string>> colors)
        {
            var parts = ColorRegex.SplitWithSeparators(data, out var _);

            var builder = new StringBuilder(data.Length);
            var offset = 0;
            var useBuilder = true;

            for (var i = 0; i < parts.Length; i++)
            {
                if (useBuilder)
                {
                    builder.Append(parts[i]);
                    offset += parts[i].Length;
                    useBuilder = false;
                }
                else
                {
                    colors.Add(new KeyValuePair<int, string>(offset, parts[i]));
                    useBuilder = true;
                }
            }

            return builder.ToString();
        }

        public static string ExtractColor(string data)
        {
            var length = data.Length;
            var builder = new StringBuilder(length);
            var dataSpan = data.AsSpan();
            var last = 0;

            for (var i = 0; i < length; i++)
            {
                if (data[i] == 0x1b && i < length - 2 && data[i + 1] == '[')
                {
                    var colorIndex = i + 2;
                    for (; colorIndex < length; colorIndex++)
                    {
                        switch (data[colorIndex])
                        {
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case ';':
                                break;
                            case 'm':
                                colorIndex++;
                                goto end;
                            default:
                                goto skip;
                        }
                    }

                end:
                    builder.Append(dataSpan[last..i]);
                    i = colorIndex;
                    last = i;
                skip:;
                }
            }

            return builder.Append(dataSpan[last..])
                .ToString();
        }
    }
}
