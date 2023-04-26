using System.Text.RegularExpressions;

namespace Wilgysef.StdoutHook.Rules
{
    public class ActivationExpression
    {
        public Regex Expression { get; set; }

        public long? ActivationOffset { get; set; }

        public long? ActivationOffsetStdoutOnly { get; set; }

        public long? ActivationOffsetStderrOnly { get; set; }

        public ActivationExpression(Regex expression, long activationOffset = 0)
        {
            Expression = expression;
            ActivationOffset = activationOffset;
        }
    }
}
