using System.Collections.Generic;
using Wilgysef.StdoutHook.Utilities;

namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class RuleDto
    {
        #region Base Rule

        public bool? Enabled { get; set; }

        public object? EnableExpression { get; set; }

        public bool? StdoutOnly { get; set; }

        public bool? StderrOnly { get; set; }

        public bool? Terminal { get; set; }

        public bool? TrimNewline { get; set; }

        public IList<long>? ActivationLines { get; set; }

        public IList<long>? ActivationLinesStdoutOnly { get; set; }

        public IList<long>? ActivationLinesStderrOnly { get; set; }

        public IList<long>? DeactivationLines { get; set; }

        public IList<long>? DeactivationLinesStdoutOnly { get; set; }

        public IList<long>? DeactivationLinesStderrOnly { get; set; }

        public IList<ActivationExpressionDto>? ActivationExpressions { get; set; }

        public IList<ActivationExpressionDto>? ActivationExpressionsStdoutOnly { get; set; }

        public IList<ActivationExpressionDto>? ActivationExpressionsStderrOnly { get; set; }

        public IList<ActivationExpressionDto>? DeactivationExpressions { get; set; }

        public IList<ActivationExpressionDto>? DeactivationExpressionsStdoutOnly { get; set; }

        public IList<ActivationExpressionDto>? DeactivationExpressionsStderrOnly { get; set; }

        public string? GetEnableExpression() => StringExpressions.GetExpression(EnableExpression);

        #endregion

        #region Field Separator Rule

        public object? SeparatorExpression { get; set; }

        public int? MinFields { get; set; }

        public int? MaxFields { get; set; }

        public object? ReplaceFields { get; set; }

        public string? GetSeparatorExpression() => StringExpressions.GetExpression(SeparatorExpression);

        #endregion

        #region Filter Rule

        public bool? Filter { get; set; }

        #endregion

        #region Regex Group Rule

        public object? Regex { get; set; }

        public object? ReplaceGroups { get; set; }

        public string? GetRegex() => StringExpressions.GetExpression(Regex);

        #endregion

        #region Tee Rule

        public string? Filename { get; set; }

        public bool? Flush { get; set; }

        public bool? ExtractColors { get; set; }

        #endregion

        #region Field Separator, Regex Group, Unconditional Replace

        public string? ReplaceAllFormat { get; set; }

        #endregion
    }
}
