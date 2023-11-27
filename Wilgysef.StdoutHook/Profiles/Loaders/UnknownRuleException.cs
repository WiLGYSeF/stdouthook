namespace Wilgysef.StdoutHook.Profiles.Loaders;

public class UnknownRuleException : ProfileLoaderException
{
    public UnknownRuleException(string rule)
        : base(rule)
    {
    }
}
