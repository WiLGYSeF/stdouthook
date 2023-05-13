using System.Collections.Generic;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders
{
    internal class ProfileFormatBuilder : PropertyFormatBuilder<ProfileState>
    {
        private static readonly Property[] ProfileProperties = new[]
        {
            new Property(new[] { "stderrLines", "stderrLinecount" }, state => state.StderrLineCount.ToString(), false),
            new Property(new[] { "lines", "linecount", "stdoutLines", "stdoutLinecount" }, state => state.StdoutLineCount.ToString(), false),
            new Property(new[] { "totalLines", "totalLineCount" }, state => state.LineCount.ToString(), false),
        };

        public override string? Key => "profile";

        public override char? KeyShort => null;

        protected override IReadOnlyList<Property> GetProperties()
        {
            return ProfileProperties;
        }

        protected override ProfileState GetValue(DataState state)
        {
            return state.ProfileState;
        }
    }
}
