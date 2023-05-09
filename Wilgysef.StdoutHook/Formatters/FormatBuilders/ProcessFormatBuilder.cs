using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ProcessFormatBuilder : FormatBuilder
    {
        private static readonly ProcessProperty[] ProcessProperties = new[]
        {
            new ProcessProperty(new[] { "basePriority", "priority" }, process => process.BasePriority.ToString(), false),
            new ProcessProperty(new[] { "id", "pid", "processId" }, process => process.Id.ToString(), true),
            new ProcessProperty(new[] { "fullpath" }, process => process.MainModule.FileName, true),
            new ProcessProperty(new[] { "nonpagedSystemMemorySize" }, process => process.NonpagedSystemMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "pagedSystemMemorySize" }, process => process.PagedSystemMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "pagedMemorySize" }, process => process.PagedMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "peakPagedMemorySize" }, process => process.PeakPagedMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "peakVirtualMemorySize" }, process => process.PeakVirtualMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "peakWorkingSet" }, process => process.PeakWorkingSet64.ToString(), false),
            new ProcessProperty(new[] { "privateMemorySize" }, process => process.PrivateMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "privilegedProcessorTime" }, (process, format) => process.PrivilegedProcessorTime.ToString(format), false),
            new ProcessProperty(new[] { "priorityClass" }, process => process.PriorityClass.ToString(), false),
            new ProcessProperty(new[] { "name", "processName" }, process => process.ProcessName.ToString(), true),
            new ProcessProperty(new[] { "start", "started", "startTime" }, (process, format) => process.StartTime.ToString(format), true),
            new ProcessProperty(new[] { "processorTime", "totalProcessorTime" }, (process, format) => process.TotalProcessorTime.ToString(format), false),
            new ProcessProperty(new[] { "userProcessorTime" }, (process, format) => process.UserProcessorTime.ToString(format), false),
            new ProcessProperty(new[] { "virtualMemorySize" }, process => process.VirtualMemorySize64.ToString(), false),
            new ProcessProperty(new[] { "workingSet" }, process => process.WorkingSet64.ToString(), false),
            new ProcessProperty(new[] { "duration" }, (process, format) => (DateTime.Now - process.StartTime).ToString(format), false),
        };

        public override string? Key => "process";

        public override char? KeyShort => null;

        public override Func<DataState, string> Build(FormatBuildState state, out bool isConstant)
        {
            foreach (var prop in ProcessProperties)
            {
                if (prop.Matches(state.Contents, out var func))
                {
                    isConstant = false;
                    return dataState => func(dataState.ProfileState.Process);
                }
            }

            throw new ArgumentException($"Invalid process property: {state.Contents}");
        }

        private class ProcessProperty
        {
            private const char Separator = ':';

            private readonly string[] _names;
            private readonly Func<Process, string>? _func;
            private readonly Func<Process, string, string>? _funcFormat;
            private readonly bool _isConstant;

            public ProcessProperty(string[] names, Func<Process, string> func, bool isConstant)
            {
                _names = names;
                _func = func;
                _isConstant = isConstant;
            }

            public ProcessProperty(string[] names, Func<Process, string, string> func, bool isConstant)
            {
                _names = names;
                _funcFormat = func;
                _isConstant = isConstant;
            }

            public bool Matches(
                string str,
                [MaybeNullWhen(false)] out Func<Process, string> func)
            {
                var separatorIndex = str.IndexOf(Separator);
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
