using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Wilgysef.StdoutHook.Loggers;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

/// <summary>
/// Format builder for properties.
/// </summary>
/// <typeparam name="T">Property type.</typeparam>
internal abstract class PropertyFormatBuilder<T> : FormatBuilder
{
    /// <inheritdoc/>
    public override Func<FormatComputeState, string> Build(FormatBuildState state, out bool isConstant)
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
                    return computeState =>
                    {
                        try
                        {
                            return func(GetValue(computeState.DataState));
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

    /// <summary>
    /// Gets the list of property names.
    /// </summary>
    /// <returns>Property names.</returns>
    protected abstract IReadOnlyList<Property> GetProperties();

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="state">Data state.</param>
    /// <returns>Value.</returns>
    protected abstract T GetValue(DataState state);

    /// <summary>
    /// Property.
    /// </summary>
    protected class Property
    {
        private readonly string[] _names;
        private readonly Func<T, string>? _func;
        private readonly Func<T, string, string>? _funcFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="names">Property names.</param>
        /// <param name="func">Format method.</param>
        /// <param name="isConstant">Indicates whether the property value is constant.</param>
        public Property(string[] names, Func<T, string> func, bool isConstant)
        {
            _names = names;
            _func = func;
            IsConstant = isConstant;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="names">Property names.</param>
        /// <param name="func">Format method.</param>
        /// <param name="isConstant">Indicates whether the property value is constant.</param>
        public Property(string[] names, Func<T, string, string> func, bool isConstant)
        {
            _names = names;
            _funcFormat = func;
            IsConstant = isConstant;
        }

        /// <summary>
        /// Indicates whether the property is constant.
        /// </summary>
        public bool IsConstant { get; }

        /// <summary>
        /// Checks if the string matches the property name.
        /// </summary>
        /// <param name="str">String to check for a match.</param>
        /// <param name="func">The format function, if <paramref name="str"/> matches the property name.</param>
        /// <returns><see langword="true"/> if <paramref name="str"/> matches, otherwise <see langword="false"/>.</returns>
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
