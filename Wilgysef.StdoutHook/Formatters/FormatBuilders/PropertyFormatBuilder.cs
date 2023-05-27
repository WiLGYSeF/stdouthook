using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal abstract class PropertyFormatBuilder<T> : FormatBuilder
    {
        protected abstract IReadOnlyList<Property> GetProperties();

        protected abstract T GetValue(DataState state);

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            foreach (var prop in GetProperties())
            {
                if (prop.Matches(state.Contents, out var func))
                {
                    if (prop.IsConstant)
                    {
                        isConstant = true;
                        var value = func(GetValue(new DataState(state.Profile)));
                        return _ => value;
                    }
                    else
                    {
                        isConstant = false;
                        return dataState =>
                        {
                            try
                            {
                                return func(GetValue(dataState));
                            }
                            catch (Exception ex)
                            {
                                GlobalLogger.Error($"failed to get property value \"{state.Contents}\": {ex.Message}");
                                return "";
                            }
                        };
                    }
                }
            }

            throw new ArgumentException($"Invalid property: {state.Contents}");
        }

        protected class Property
        {
            public bool IsConstant { get; }

            private readonly string[] _names;
            private readonly Func<T, string>? _func;
            private readonly Func<T, string, string>? _funcFormat;

            public Property(string[] names, Func<T, string> func, bool isConstant)
            {
                _names = names;
                _func = func;
                IsConstant = isConstant;
            }

            public Property(string[] names, Func<T, string, string> func, bool isConstant)
            {
                _names = names;
                _funcFormat = func;
                IsConstant = isConstant;
            }

            public bool Matches(
                string str,
                [MaybeNullWhen(false)] out Func<T, string> func)
            {
                var separatorIndex = str.IndexOf(Formatter.Separator);
                var format = "";

                if (separatorIndex != -1)
                {
                    format = str[(separatorIndex + 1)..];
                    str = str[..separatorIndex];
                }

                foreach (var name in _names)
                {
                    if (str.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        func = _funcFormat != null
                            ? process => _funcFormat(process, format)
                            : _func!;
                        return true;
                    }
                }

                func = null;
                return false;
            }
        }
    }
}
