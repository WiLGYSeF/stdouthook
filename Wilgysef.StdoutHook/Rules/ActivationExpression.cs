using System;
using System.Text.RegularExpressions;

namespace Wilgysef.StdoutHook.Rules;

public class ActivationExpression
{
    public ActivationExpression(Regex expression, long activationOffset = 0)
    {
        if (activationOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(activationOffset), "Activation offset cannot be negative.");
        }

        Expression = expression;
        ActivationOffset = activationOffset;
    }

    public Regex Expression { get; set; }

    public long ActivationOffset { get; set; }
}
