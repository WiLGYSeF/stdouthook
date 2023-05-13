using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class ProfileDto
    {
        public string? ProfileName { get; set; }

        public string? Command { get; set; }

        public string? CommandExpression { get; set; }

        public string? FullCommandPath { get; set; }

        public string? FullCommandPathExpression { get; set; }

        public bool? CommandIgnoreCase { get; set; }

        public bool? Enabled { get; set; }

        public bool? PseudoTty { get; set; }

        public bool? Flush { get; set; }

        public IList<RuleDto>? Rules { get; set; }

        public IDictionary<string, string>? CustomColors { get; set; }

        public IList<string>? InheritProfileNames { get; set; }

    }
}
