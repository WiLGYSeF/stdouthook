using System;
using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class Formatter
    {
        public bool InvalidFormatBlank { get; set; }

        private readonly FormatFunctionBuilder _formatFunctionBuilder;

        public Formatter(FormatFunctionBuilder formatFunctionBuilder)
        {
            _formatFunctionBuilder = formatFunctionBuilder;
        }

        public CompiledFormat CompileFormat(string format)
        {
            var parts = new List<string>();
            var funcs = new List<Func<string>>();
            var last = 0;
            var lastPart = "";

            for (var i = 0; i < format.Length; i++)
            {
                if (format[i] == '%' && i < format.Length - 1)
                {
                    if (format[i + 1] == '%')
                    {
                        lastPart += format[last..i];
                        last = ++i;
                        continue;
                    }

                    if (format[i + 1] == '('
                        || i < format.Length - 2 && IsKeyChar(format[i + 1]) && format[i + 2] == '(')
                    {
                        string? key = (format[i + 1] == '(' ? null : format[i + 1].ToString());
                        var start = i + (format[i + 1] == '(' ? 2 : 3);
                        var endIndex = start;
                        var count = 1;

                        for (; endIndex < format.Length; endIndex++)
                        {
                            if (format[endIndex] == '(')
                            {
                                count++;
                            }
                            else if (format[endIndex] == ')')
                            {
                                count--;
                                if (count == 0)
                                {
                                    break;
                                }
                            }
                        }

                        if (endIndex < format.Length)
                        {
                            var contents = format[start..endIndex];
                            if (key == null)
                            {
                                var k = 0;
                                for (; k < contents.Length; k++)
                                {
                                    if (!IsNameChar(contents[k]))
                                    {
                                        break;
                                    }
                                }

                                key = contents[..k];
                                contents = contents[k..];
                            }

                            if (key.Length > 0)
                            {
                                BuildFormat(key, contents, i, endIndex + 1);
                            }
                            else
                            {
                                lastPart += format[last..(endIndex + 1)];
                                last = endIndex + 1;
                            }
                        }

                        i = endIndex;
                    }
                    else
                    {
                        var start = i + 1;
                        var endIndex = start;

                        for (; endIndex < format.Length; endIndex++)
                        {
                            if (!IsNameChar(format[endIndex]))
                            {
                                break;
                            }
                        }

                        if (endIndex != start)
                        {
                            BuildFormat(format[start..endIndex], "", i, endIndex);
                        }
                        else
                        {
                            lastPart += format[last..endIndex];
                            last = endIndex;
                        }

                        i = endIndex;
                    }
                }
            }

            parts.Add(lastPart + format[last..]);

            return new CompiledFormat(parts.ToArray(), funcs.ToArray());

            void BuildFormat(string key, string contents, int startIndex, int endIndex)
            {
                try
                {
                    var func = _formatFunctionBuilder.Build(key, contents, out var isConstant);

                    if (isConstant)
                    {
                        lastPart += format[last..startIndex] + func();
                    }
                    else
                    {
                        parts.Add(lastPart + format[last..startIndex]);
                        funcs.Add(func);
                        lastPart = "";
                    }
                }
                catch
                {
                    var end = InvalidFormatBlank ? startIndex : endIndex;
                    lastPart += format[last..end];
                }

                last = endIndex;
            }
        }

        public string Format(string format)
        {
            return CompileFormat(format).ToString();
        }

        private static bool IsNameChar(char c)
        {
            switch (c)
            {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
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
                case '_':
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsKeyChar(char c)
        {
            switch (c)
            {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    return true;
                default:
                    return false;
            }
        }
    }
}
