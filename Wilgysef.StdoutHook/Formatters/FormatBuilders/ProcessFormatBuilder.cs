using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, string> _startTimeCache = new ConcurrentDictionary<string, string>();

        private string _processId = null!;
        private string _filename = null!;
        private DateTime _startTime;
        private bool _cached;

        public ProcessFormatBuilder()
        {
            _durationStopwatch = new OffsetStopwatch();

            _processProperties = new[]
            {
                // all properties are not constant because formatters are built before the process starts
                new Property(new[] { "basePriority", "priority" }, process => process.BasePriority.ToString(), false),
                new Property(new[] { "id", "pid", "processId" }, _ => _processId!, false),
                new Property(new[] { "fullpath" }, _ => _filename!, false),
                new Property(new[] { "nonpagedSystemMemorySize" }, process => process.NonpagedSystemMemorySize64.ToString(), false),
                new Property(new[] { "pagedSystemMemorySize" }, process => process.PagedSystemMemorySize64.ToString(), false),
                new Property(new[] { "pagedMemorySize" }, process => process.PagedMemorySize64.ToString(), false),
                new Property(new[] { "peakPagedMemorySize" }, process => process.PeakPagedMemorySize64.ToString(), false),
                new Property(new[] { "peakVirtualMemorySize" }, process => process.PeakVirtualMemorySize64.ToString(), false),
                new Property(new[] { "peakWorkingSet" }, process => process.PeakWorkingSet64.ToString(), false),
                new Property(new[] { "privateMemorySize" }, process => process.PrivateMemorySize64.ToString(), false),
                new Property(new[] { "privilegedProcessorTime" }, (process, format) => process.PrivilegedProcessorTime.ToString(format), false),
                new Property(new[] { "priorityClass" }, process => process.PriorityClass.ToString(), false),
                new Property(new[] { "name", "processName" }, process => process.ProcessName, false),
                new Property(new[] { "start", "started", "startTime" }, (process, format) => _startTimeCache.GetOrAdd(format, key => _startTime.ToString(key)), false),
                new Property(new[] { "processorTime", "totalProcessorTime" }, (process, format) => process.TotalProcessorTime.ToString(format), false),
                new Property(new[] { "userProcessorTime" }, (process, format) => process.UserProcessorTime.ToString(format), false),
                new Property(new[] { "virtualMemorySize" }, process => process.VirtualMemorySize64.ToString(), false),
                new Property(new[] { "workingSet" }, process => process.WorkingSet64.ToString(), false),
                new Property(new[] { "duration" }, (_, format) => _durationStopwatch.Elapsed.ToString(format), false),
            };
        }

        protected override IReadOnlyList<Property> GetProperties()
        {
            return _processProperties;
        }

        protected override Process GetValue(DataState state)
        {
            var process = state.Profile.State.Process;

            if (!_cached)
            {
                _processId = process.Id.ToString();
                _filename = process.MainModule.FileName;
                _startTime = process.StartTime;

                _durationStopwatch.Offset = DateTime.Now - _startTime;
                _durationStopwatch.Start();

                _cached = true;
            }

            return process;
        }
    }
}
