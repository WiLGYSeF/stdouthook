using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ProcessFormatBuilder : PropertyFormatBuilder<Process>
    {
        private static readonly Property[] ProcessProperties = new[]
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
            new Property(new[] { "start", "started", "startTime" }, (process, format) => process.StartTime.ToString(format), false),
            new Property(new[] { "processorTime", "totalProcessorTime" }, (process, format) => process.TotalProcessorTime.ToString(format), false),
            new Property(new[] { "userProcessorTime" }, (process, format) => process.UserProcessorTime.ToString(format), false),
            new Property(new[] { "virtualMemorySize" }, process => process.VirtualMemorySize64.ToString(), false),
            new Property(new[] { "workingSet" }, process => process.WorkingSet64.ToString(), false),
            new Property(new[] { "duration" }, (process, format) => (DateTime.Now - process.StartTime).ToString(format), false),
        };

        public override string? Key => "process";

        public override char? KeyShort => null;

        protected override IReadOnlyList<Property> GetProperties()
        {
            return ProcessProperties;
        }

        protected override Process GetValue(DataState state)
        {
            return state.Profile.State!.Process;
        }
    }
}
