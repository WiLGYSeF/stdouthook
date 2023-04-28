using System;
using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Formatters
{
    internal class Formatter
    {
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
                if (format[i] == '\\')
                {
                    i++;
                    continue;
                }

                if (format[i] == '%' && i < format.Length - 1)
                {
                    if (format[i + 1] == '('
                        || i < format.Length - 2 && IsKeyChar(format[i + 1]) && format[i + 2] == '(')
                    {
                        var count = 1;
                        string? key = (format[i + 1] == '(' ? null : format[i + 1].ToString());
                        var start = i + (format[i + 1] == '(' ? 2 : 3);
                        var j = start;

                        for (; j < format.Length; j++)
                        {
                            if (format[j] == '(')
                            {
                                count++;
                            }
                            else if (format[j] == ')')
                            {
                                count--;
                                if (count == 0)
                                {
                                    break;
                                }
                            }
                        }

                        if (j < format.Length)
                        {
                            var contents = format[start..j];
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

                            try
                            {
                                var func = _formatFunctionBuilder.Build(key, contents, out var isConstant);

                                if (isConstant)
                                {
                                    lastPart += format[last..i] + func();
                                }
                                else
                                {
                                    parts.Add(lastPart + format[last..i]);
                                    funcs.Add(func);
                                    lastPart = "";
                                }
                            }
                            catch
                            {
                                lastPart += format[last..i];
                            }

                            last = j + 1;
                        }

                        i = j;
                    }
                    else
                    {
                        var start = i + 1;
                        var j = start;

                        for (; j < format.Length; j++)
                        {
                            if (!IsNameChar(format[j]))
                            {
                                break;
                            }
                        }

                        try
                        {
                            var func = _formatFunctionBuilder.Build(format[start..j], "", out var isConstant);

                            if (isConstant)
                            {
                                lastPart += format[last..i] + func();
                            }
                            else
                            {
                                parts.Add(lastPart + format[last..i]);
                                funcs.Add(func);
                                lastPart = "";
                            }
                        }
                        catch
                        {
                            lastPart += format[last..i];
                        }

                        last = j;
                        i = j;
                    }
                }
            }

            parts.Add(lastPart + format[last..]);

            return new CompiledFormat(parts.ToArray(), funcs.ToArray());
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
                case '_': // TODO: keep?
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
