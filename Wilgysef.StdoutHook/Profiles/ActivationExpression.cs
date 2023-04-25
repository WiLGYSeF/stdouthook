using System.Text.RegularExpressions;

namespace Wilgysef.StdoutHook.Profiles
{
    public class ActivationExpression
    {
        public Regex Expression { get; set; }

        public int ActivationOffset { get; set; }

        public ActivationExpression(Regex expression, int activationOffset = 0)
        {
            Expression = expression;
            ActivationOffset = activationOffset;
        }
    }
}
