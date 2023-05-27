using System.Collections.Generic;
using Wilgysef.StdoutHook.Utilities;

namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class ProfileDto
    {
        public bool? Enabled { get; set; }

        #region Picker Properties

        public string? ProfileName { get; set; }

        public string? Command { get; set; }

        public object? CommandExpression { get; set; }

        public string? FullCommandPath { get; set; }

        public object? FullCommandPathExpression { get; set; }

        public bool? CommandIgnoreCase { get; set; }

        public object? Subcommand { get; set; }

        public object? SubcommandExpression { get; set; }

        public IList<ArgumentPatternDto>? ArgumentPatterns { get; set; }

        public int? MinArguments { get; set; }

        public int? MaxArguments { get; set; }

        #endregion

        public bool? PseudoTty { get; set; }

        public bool? Flush { get; set; }

        public int? BufferSize { get; set; }

        public int? OutputFlushInterval { get; set; }

        public bool? Interactive { get; set; }

        public int? InteractiveFlushInterval { get; set; }

        public IList<RuleDto>? Rules { get; set; }

        public IDictionary<string, string>? CustomColors { get; set; }

        public IList<string>? InheritProfiles { get; set; }

        public string? GetCommandExpression() => StringExpressions.GetExpression(CommandExpression);

        public string? GetFullCommandPathExpression() => StringExpressions.GetExpression(FullCommandPathExpression);

        public string? GetSubcommandExpression() => StringExpressions.GetExpression(SubcommandExpression);
    }
}
