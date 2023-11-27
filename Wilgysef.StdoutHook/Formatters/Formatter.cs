using System;
using System.Collections.Generic;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters;

/// <summary>
/// Formatter.
/// </summary>
internal class Formatter
{
    /// <summary>
    /// Key-value format separator.
    /// </summary>
    public static readonly char Separator = ':';

    private readonly FormatFunctionBuilder _formatFunctionBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="Formatter"/> class.
    /// </summary>
    /// <param name="formatFunctionBuilder">Format function builder.</param>
    public Formatter(FormatFunctionBuilder formatFunctionBuilder)
    {
        _formatFunctionBuilder = formatFunctionBuilder;
    }

    /// <summary>
    /// Indicates if invalid format values should be replaced with empty strings.
    /// </summary>
    public bool InvalidFormatBlank { get; set; }

    /// <summary>
    /// Compiles the format <paramref name="format"/>.
    /// </summary>
    /// <remarks>
    /// Formatters are in the format <c>%formatKey</c>, <c>%(formatKey)</c>, or <c>%(formatKey:formatValue)</c>.
    /// </remarks>
    /// <param name="format">Format to compile.</param>
    /// <param name="profile">Profile.</param>
    /// <returns>Compiled format.</returns>
    public CompiledFormat CompileFormat(string format, Profile profile)
    {
        var parts = new List<string>();
        var funcs = new List<Func<FormatComputeState, string>>();
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

                var nextCharIsParen = format[i + 1] == '(';
                if (nextCharIsParen
                    || (i < format.Length - 2 && IsKeyChar(format[i + 1]) && format[i + 2] == '('))
                {
                    var key = format.AsSpan(i + 1, nextCharIsParen ? 0 : 1);
                    var start = i + (nextCharIsParen ? 2 : 3);
                    var endIndex = start;

                    for (var count = 1; endIndex < format.Length; endIndex++)
                    {
                        if (format[endIndex] == '(')
                        {
                            count++;
                        }
                        else if (format[endIndex] == ')')
                        {
                            if (--count == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (endIndex < format.Length)
                    {
                        var contents = format.AsSpan(start, endIndex - start);
                        if (key.Length == 0)
                        {
                            var k = 0;
                            var validKey = true;

                            for (; k < contents.Length; k++)
                            {
                                if (!IsNameChar(contents[k]))
                                {
                                    validKey = contents[k] == Separator;
                                    break;
                                }
                            }

                            if (k == contents.Length)
                            {
                                key = contents;
                                contents = "";
                            }
                            else if (validKey)
                            {
                                key = contents[..k];
                                contents = contents[(k + 1)..];
                            }
                        }

                        if (key.Length > 0)
                        {
                            BuildFormat(key.ToString(), contents.ToString(), i, endIndex + 1);
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

                    i = endIndex - 1;
                }
            }
        }

        parts.Add(lastPart + format[last..]);

        return new CompiledFormat(parts.ToArray(), funcs.ToArray());

        void BuildFormat(string key, string contents, int startIndex, int endIndex)
        {
            try
            {
                var func = _formatFunctionBuilder.Build(key, contents, profile, out var isConstant);

                if (isConstant)
                {
                    lastPart += format[last..startIndex] + func(new FormatComputeState(new DataState(profile)));
                }
                else
                {
                    parts.Add(lastPart + format[last..startIndex]);
                    funcs.Add(func);
                    lastPart = "";
                }
            }
            catch (Exception ex)
            {
                GlobalLogger.Error($"Invalid format: {ex.Message}");

                var end = InvalidFormatBlank ? startIndex : endIndex;
                lastPart += format[last..end];
            }

            last = endIndex;
        }
    }

    /// <summary>
    /// Compile and compute format <paramref name="format"/>.
    /// </summary>
    /// <param name="format">Format.</param>
    /// <param name="state">Data state.</param>
    /// <returns>Formatted string.</returns>
    public string Format(string format, DataState state)
    {
        return CompileFormat(format, state.Profile).Compute(state);
    }

    private static bool IsNameChar(char c)
    {
        return c switch
        {
            'A' or 'B' or 'C' or 'D' or 'E' or 'F' or 'G' or 'H' or 'I' or 'J' or 'K' or 'L' or 'M' or 'N' or 'O' or 'P' or 'Q' or 'R' or 'S' or 'T' or 'U' or 'V' or 'W' or 'X' or 'Y' or 'Z' or 'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'g' or 'h' or 'i' or 'j' or 'k' or 'l' or 'm' or 'n' or 'o' or 'p' or 'q' or 'r' or 's' or 't' or 'u' or 'v' or 'w' or 'x' or 'y' or 'z' or '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' or '_' => true,
            _ => false,
        };
    }

    private static bool IsKeyChar(char c)
    {
        return c switch
        {
            'A' or 'B' or 'C' or 'D' or 'E' or 'F' or 'G' or 'H' or 'I' or 'J' or 'K' or 'L' or 'M' or 'N' or 'O' or 'P' or 'Q' or 'R' or 'S' or 'T' or 'U' or 'V' or 'W' or 'X' or 'Y' or 'Z' or 'a' or 'b' or 'c' or 'd' or 'e' or 'f' or 'g' or 'h' or 'i' or 'j' or 'k' or 'l' or 'm' or 'n' or 'o' or 'p' or 'q' or 'r' or 's' or 't' or 'u' or 'v' or 'w' or 'x' or 'y' or 'z' => true,
            _ => false,
        };
    }
}
