using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class InvalidRuleException : ProfileLoaderException
    {
        public Rule Rule { get; }

        public InvalidRuleException(Rule rule, string message) : base($"Invalid rule: {message}")
        {
            Rule = rule;
        }
    }
}
