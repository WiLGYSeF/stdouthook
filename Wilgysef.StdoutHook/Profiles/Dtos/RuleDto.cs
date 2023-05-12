using System.Collections.Generic;

namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class RuleDto
    {
        #region Base Rule

        public bool? Enabled { get; set; }

        public string? EnableExpression { get; set; }

        public bool? StdoutOnly { get; set; }

        public bool? StderrOnly { get; set; }

        public bool? Terminal { get; set; }

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

        #endregion

        #region Field Separator Rule

        public string? SeparatorExpression { get; set; }

        public int? MinFields { get; set; }

        public int? MaxFields { get; set; }

        public object? ReplaceFields { get; set; }

        #endregion

        #region Filter Rule

        public bool? Filter { get; set; }

        #endregion

        #region Regex Group Rule

        public string? Regex { get; set; }

        public object? ReplaceGroups { get; set; }

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
