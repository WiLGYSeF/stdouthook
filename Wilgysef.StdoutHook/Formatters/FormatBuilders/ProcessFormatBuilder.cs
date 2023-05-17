using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Utilities;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ProcessFormatBuilder : PropertyFormatBuilder<Process>
    {
        public override string? Key => "process";

        public override char? KeyShort => null;

        private readonly Property[] _processProperties;
        private readonly OffsetStopwatch _durationStopwatch;

        private DateTime? _startTime;

        public ProcessFormatBuilder()
        {
            _durationStopwatch = new OffsetStopwatch();

            _processProperties = new[]
            {
                // all properties are not constant because formatters are built before the process starts
                new Property(new[] { "basePriority", "priority" }, process => process.BasePriority.ToString(), false),
                new Property(new[] { "id", "pid", "processId" }, process => process.Id.ToString(), false),
                new Property(new[] { "fullpath" }, process => process.MainModule.FileName, false),
                new Property(new[] { "nonpagedSystemMemorySize" }, process => process.NonpagedSystemMemorySize64.ToString(), false),
                new Property(new[] { "pagedSystemMemorySize" }, process => process.PagedSystemMemorySize64.ToString(), false),
                new Property(new[] { "pagedMemorySize" }, process => process.PagedMemorySize64.ToString(), false),
                new Property(new[] { "peakPagedMemorySize" }, process => process.PeakPagedMemorySize64.ToString(), false),
                new Property(new[] { "peakVirtualMemorySize" }, process => process.PeakVirtualMemorySize64.ToString(), false),
                new Property(new[] { "peakWorkingSet" }, process => process.PeakWorkingSet64.ToString(), false),
                new Property(new[] { "privateMemorySize" }, process => process.PrivateMemorySize64.ToString(), false),
                new Property(new[] { "privilegedProcessorTime" }, (process, format) => process.PrivilegedProcessorTime.ToString(format), false),
                new Property(new[] { "priorityClass" }, process => process.PriorityClass.ToString(), false),
                new Property(new[] { "name", "processName" }, process => process.ProcessName.ToString(), false),
                new Property(new[] { "start", "started", "startTime" }, (process, format) => _startTime!.Value.ToString(format), false),
                new Property(new[] { "processorTime", "totalProcessorTime" }, (process, format) => process.TotalProcessorTime.ToString(format), false),
                new Property(new[] { "userProcessorTime" }, (process, format) => process.UserProcessorTime.ToString(format), false),
                new Property(new[] { "virtualMemorySize" }, process => process.VirtualMemorySize64.ToString(), false),
                new Property(new[] { "workingSet" }, process => process.WorkingSet64.ToString(), false),
                new Property(new[] { "duration" }, (process, format) => _durationStopwatch.Elapsed.ToString(format), false),
            };
        }

        protected override IReadOnlyList<Property> GetProperties()
        {
            return _processProperties;
        }

        protected override Process GetValue(DataState state)
        {
            var process = state.Profile.State.Process;

            _startTime ??= process.StartTime;

            if (!_durationStopwatch.IsRunning)
            {
                _durationStopwatch.Offset = DateTime.Now - _startTime!.Value;
                _durationStopwatch.Start();
            }

            return process;
        }
    }
}
