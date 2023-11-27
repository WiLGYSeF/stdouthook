using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles.Loaders;

public class InvalidRuleException : ProfileLoaderException
{
    public InvalidRuleException(Rule rule, string message)
        : base($"Invalid rule: {message}")
    {
        Rule = rule;
    }

    public Rule Rule { get; }
}
