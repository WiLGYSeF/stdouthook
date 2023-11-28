using Wilgysef.StdoutHook.Utilities;

namespace Wilgysef.StdoutHook.Profiles.Dtos;

public class ArgumentPatternDto
{
    public bool? Enabled { get; set; }

    public object? ArgumentExpression { get; set; }

    public int? MinPosition { get; set; }

    public int? MaxPosition { get; set; }

    public bool? MustNotMatch { get; set; }

    public string? GetArgumentExpression() => StringExpressions.GetExpression(ArgumentExpression);
}
