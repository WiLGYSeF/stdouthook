using Wilgysef.StdoutHook.Utilities;

namespace Wilgysef.StdoutHook.Profiles.Dtos
{
    public class ActivationExpressionDto
    {
        public object? Expression { get; set; }

        public long? ActivationOffset { get; set; }

        public string? GetExpression() => StringExpressions.GetExpression(Expression);
    }
}
