using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class ProfileDto
    {
        public bool? Enabled { get; set; }

        public string? ProfileName { get; set; }

        public string? Command { get; set; }

        public string? CommandExpression { get; set; }

        public string? FullCommandPath { get; set; }

        public string? FullCommandPathExpression { get; set; }

        public bool? CommandIgnoreCase { get; set; }

        public object? Subcommand { get; set; }

        public string? SubcommandExpression { get; set; }

        public IList<ArgumentPatternDto>? ArgumentPatterns { get; set; }

        public int? MinArguments { get; set; }

        public int? MaxArguments { get; set; }

        public bool? PseudoTty { get; set; }

        public bool? Flush { get; set; }

        public IList<RuleDto>? Rules { get; set; }

        public IDictionary<string, string>? CustomColors { get; set; }

        public IList<string>? InheritProfileNames { get; set; }
    }
}
