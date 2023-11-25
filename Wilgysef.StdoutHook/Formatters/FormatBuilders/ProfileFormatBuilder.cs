using System.Collections.Generic;
using Wilgysef.StdoutHook.Profiles;

namespace Wilgysef.StdoutHook.Formatters.FormatBuilders;

internal class ProfileFormatBuilder : PropertyFormatBuilder<Profile>
{
    private static readonly Property[] ProfileProperties = new[]
    {
        new Property(new[] { "name", "profileName" }, profile => profile.ProfileName ?? "", true),

        new Property(new[] { "stderrLines", "stderrLinecount" }, (profile, format) => profile.State.StderrLineCount.ToString(format), false),
        new Property(new[] { "lines", "linecount", "stdoutLines", "stdoutLinecount" }, (profile, format) => profile.State.StdoutLineCount.ToString(format), false),
        new Property(new[] { "totalLines", "totalLineCount" }, (profile, format) => profile.State.LineCount.ToString(format), false),
    };

    public override string? Key => "profile";

    public override char? KeyShort => null;

    protected override IReadOnlyList<Property> GetProperties()
    {
        return ProfileProperties;
    }

    protected override Profile GetValue(DataState state)
    {
        return state.Profile;
    }
}
