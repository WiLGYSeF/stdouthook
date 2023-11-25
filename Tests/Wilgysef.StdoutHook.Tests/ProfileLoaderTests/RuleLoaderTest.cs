using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class RuleLoaderTest
{
    #region Base Rule

    [Fact]
    public void EnableExpression()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            EnableExpression = "a",
        });

        rule.EnableExpression!.ToString().ShouldBe("a");
    }

    [Fact]
    public void EnableExpression_Split()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            EnableExpression = new List<object?> { "a", "b" },
        });

        rule.EnableExpression!.ToString().ShouldBe("ab");
    }

    [Fact]
    public void StdoutOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            StdoutOnly = true,
        });

        rule.StdoutOnly.ShouldBeTrue();
    }

    [Fact]
    public void StderrOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            StderrOnly = true,
        });

        rule.StderrOnly.ShouldBeTrue();
    }

    [Fact]
    public void Terminal()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            Terminal = true,
        });

        rule.Terminal.ShouldBeTrue();
    }

    [Fact]
    public void TrimNewline()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            TrimNewline = true,
        });

        rule.TrimNewline.ShouldBeTrue();
    }

    [Fact]
    public void ActivationLines()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            ActivationLines = new List<long> { 5 },
        });

        rule.ActivationLines.Count.ShouldBe(1);
        rule.ActivationLines.First().ShouldBe(5);
    }

    [Fact]
    public void ActivationLinesStdoutOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            ActivationLinesStdoutOnly = new List<long> { 5 },
        });

        rule.ActivationLinesStdoutOnly.Count.ShouldBe(1);
        rule.ActivationLinesStdoutOnly.First().ShouldBe(5);
    }

    [Fact]
    public void ActivationLinesStderrOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            ActivationLinesStderrOnly = new List<long> { 5 },
        });

        rule.ActivationLinesStderrOnly.Count.ShouldBe(1);
        rule.ActivationLinesStderrOnly.First().ShouldBe(5);
    }

    [Fact]
    public void DeactivationLines()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            DeactivationLines = new List<long> { 5 },
        });

        rule.DeactivationLines.Count.ShouldBe(1);
        rule.DeactivationLines.First().ShouldBe(5);
    }

    [Fact]
    public void DeactivationLinesStdoutOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            DeactivationLinesStdoutOnly = new List<long> { 5 },
        });

        rule.DeactivationLinesStdoutOnly.Count.ShouldBe(1);
        rule.DeactivationLinesStdoutOnly.First().ShouldBe(5);
    }

    [Fact]
    public void DeactivationLinesStderrOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            DeactivationLinesStderrOnly = new List<long> { 5 },
        });

        rule.DeactivationLinesStderrOnly.Count.ShouldBe(1);
        rule.DeactivationLinesStderrOnly.First().ShouldBe(5);
    }

    [Fact]
    public void ActivationExpressions()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            ActivationExpressions = new List<ActivationExpressionDto>
            {
                new ActivationExpressionDto
                {
                    Expression = "a",
                    ActivationOffset = 2,
                },
                new ActivationExpressionDto
                {
                },
            },
        });

        rule.ActivationExpressions.Count.ShouldBe(1);
        var expression = rule.ActivationExpressions.First();
        expression.Expression.ToString().ShouldBe("a");
        expression.ActivationOffset.ShouldBe(2);
    }

    [Fact]
    public void ActivationExpressionsStdoutOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            ActivationExpressionsStdoutOnly = new List<ActivationExpressionDto>
            {
                new ActivationExpressionDto
                {
                    Expression = "a",
                    ActivationOffset = 2,
                },
            },
        });

        rule.ActivationExpressionsStdoutOnly.Count.ShouldBe(1);
        var expression = rule.ActivationExpressionsStdoutOnly.First();
        expression.Expression.ToString().ShouldBe("a");
        expression.ActivationOffset.ShouldBe(2);
    }

    [Fact]
    public void ActivationExpressionsStderrOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            ActivationExpressionsStderrOnly = new List<ActivationExpressionDto>
            {
                new ActivationExpressionDto
                {
                    Expression = "a",
                    ActivationOffset = 2,
                },
            },
        });

        rule.ActivationExpressionsStderrOnly.Count.ShouldBe(1);
        var expression = rule.ActivationExpressionsStderrOnly.First();
        expression.Expression.ToString().ShouldBe("a");
        expression.ActivationOffset.ShouldBe(2);
    }

    [Fact]
    public void DeactivationExpressions()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            DeactivationExpressions = new List<ActivationExpressionDto>
            {
                new ActivationExpressionDto
                {
                    Expression = "a",
                    ActivationOffset = 2,
                },
            },
        });

        rule.DeactivationExpressions.Count.ShouldBe(1);
        var expression = rule.DeactivationExpressions.First();
        expression.Expression.ToString().ShouldBe("a");
        expression.ActivationOffset.ShouldBe(2);
    }

    [Fact]
    public void DeactivationExpressionsStdoutOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            DeactivationExpressionsStdoutOnly = new List<ActivationExpressionDto>
            {
                new ActivationExpressionDto
                {
                    Expression = "a",
                    ActivationOffset = 2,
                },
            },
        });

        rule.DeactivationExpressionsStdoutOnly.Count.ShouldBe(1);
        var expression = rule.DeactivationExpressionsStdoutOnly.First();
        expression.Expression.ToString().ShouldBe("a");
        expression.ActivationOffset.ShouldBe(2);
    }

    [Fact]
    public void DeactivationExpressionsStderrOnly()
    {
        var rule = LoadBaseRule(new RuleDto
        {
            DeactivationExpressionsStderrOnly = new List<ActivationExpressionDto>
            {
                new ActivationExpressionDto
                {
                    Expression = "a",
                    ActivationOffset = 2,
                },
            },
        });

        rule.DeactivationExpressionsStderrOnly.Count.ShouldBe(1);
        var expression = rule.DeactivationExpressionsStderrOnly.First();
        expression.Expression.ToString().ShouldBe("a");
        expression.ActivationOffset.ShouldBe(2);
    }

    [Fact]
    public void MultipleMatchingTypes()
    {
        Should.Throw<UnknownRuleException>(() => LoadBaseRule(new RuleDto
        {
            SeparatorExpression = "a",
            Regex = "b",
        }));
    }

    private static Rule LoadBaseRule(RuleDto dto)
    {
        var loader = new RuleLoader();

        dto.SeparatorExpression = @"\s+";
        dto.ReplaceFields = new List<object?> { "a", "b" };

        return loader.LoadRule(dto);
    }

    #endregion

    #region Field Separator Rule

    [Fact]
    public void FieldSeparator_ReplaceFields_List()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new List<object?> { "a", "b" },
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.ReplaceFields!.Count.ShouldBe(2);
        rule.ReplaceFields[0].Key.ToString().ShouldBe("1");
        rule.ReplaceFields[0].Value.ShouldBe("a");
        rule.ReplaceFields[1].Key.ToString().ShouldBe("2");
        rule.ReplaceFields[1].Value.ShouldBe("b");
    }

    [Fact]
    public void FieldSeparator_ReplaceFields_Object()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new Dictionary<string, object?>
            {
                ["1"] = "a",
                ["2"] = "b",
            },
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.ReplaceFields!.Count.ShouldBe(2);
        rule.ReplaceFields[0].Key.ToString().ShouldBe("1");
        rule.ReplaceFields[0].Value.ShouldBe("a");
        rule.ReplaceFields[1].Key.ToString().ShouldBe("2");
        rule.ReplaceFields[1].Value.ShouldBe("b");
    }

    [Fact]
    public void FieldSeparator_ReplaceAllFormat()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceAllFormat = "test",
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.ReplaceAllFormat.ShouldBe("test");
    }

    [Fact]
    public void FieldSeparator_Range()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            MinFields = 1,
            MaxFields = 4,
            ReplaceAllFormat = "test",
        });

        rule.SeparatorExpression.ToString().ShouldBe(@"\s+");

        rule.MinFields.ShouldBe(1);
        rule.MaxFields.ShouldBe(4);
    }

    [Fact]
    public void FieldSeparator_SeparatorExpression_Split()
    {
        var loader = new RuleLoader();
        var rule = (FieldSeparatorRule)loader.LoadRule(new RuleDto
        {
            SeparatorExpression = new List<object?> { "a", "b" },
            ReplaceAllFormat = "test",
        });

        rule.SeparatorExpression.ToString().ShouldBe("ab");
    }

    [Fact]
    public void FieldSeparator_Invalid()
    {
        var loader = new RuleLoader();

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new List<object?> { "a", 1 },
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            SeparatorExpression = @"\s+",
            ReplaceFields = new Dictionary<string, object?>
            {
                ["a"] = "1",
                ["b"] = 2,
            },
        }));
    }

    #endregion

    #region Filter Rule

    [Fact]
    public void Filter()
    {
        var loader = new RuleLoader();
        _ = (FilterRule)loader.LoadRule(new RuleDto
        {
            Filter = true,
        });
    }

    #endregion

    #region Regex Group Rule

    [Fact]
    public void RegexGroup_ReplaceGroups_List()
    {
        var loader = new RuleLoader();
        var rule = (RegexGroupRule)loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new List<object?> { "a", "b", "c" },
        });

        rule.Regex.ToString().ShouldBe("test");

        rule.ReplaceGroups!.Count.ShouldBe(3);
    }

    [Fact]
    public void RegexGroup_ReplaceGroups_Object()
    {
        var loader = new RuleLoader();
        var rule = (RegexGroupRule)loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new Dictionary<string, object?>
            {
                ["1"] = "a",
                ["group"] = "b",
            },
        });

        rule.Regex.ToString().ShouldBe("test");

        rule.ReplaceGroups!.Count.ShouldBe(1);
        rule.ReplaceGroups[0].Key.ToString().ShouldBe("1");
        rule.ReplaceGroups[0].Value.ShouldBe("a");

        rule.ReplaceNamedGroups!.Count.ShouldBe(1);
        rule.ReplaceNamedGroups["group"].ShouldBe("b");
    }

    [Fact]
    public void RegexGroup_Invalid()
    {
        var loader = new RuleLoader();

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            Regex = "test",
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new List<object?> { "a", 2 },
        }));

        Should.Throw<InvalidRuleException>(() => loader.LoadRule(new RuleDto
        {
            Regex = "test",
            ReplaceGroups = new Dictionary<object, object?>
            {
                ["a"] = "1",
                ["b"] = 2,
            },
        }));
    }

    #endregion

    #region Tee Rule

    [Fact]
    public void Tee()
    {
        var loader = new RuleLoader();
        var rule = (TeeRule)loader.LoadRule(new RuleDto
        {
            Filename = "a",
        });

        rule.Filename.ShouldBe("a");
    }

    [Fact]
    public void Tee_Flush()
    {
        var loader = new RuleLoader();
        var rule = (TeeRule)loader.LoadRule(new RuleDto
        {
            Filename = "a",
            Flush = true,
        });

        rule.Filename.ShouldBe("a");
        rule.Flush.ShouldBeTrue();
    }

    [Fact]
    public void Tee_ExtractColors()
    {
        var loader = new RuleLoader();
        var rule = (TeeRule)loader.LoadRule(new RuleDto
        {
            Filename = "a",
            ExtractColors = true,
        });

        rule.Filename.ShouldBe("a");
        rule.ExtractColors.ShouldBeTrue();
    }

    #endregion

    #region Unconditional Replace Rule

    [Fact]
    public void UnconditionalReplace()
    {
        var loader = new RuleLoader();
        var rule = (UnconditionalReplaceRule)loader.LoadRule(new RuleDto
        {
            ReplaceAllFormat = "a",
        });

        rule.Format.ShouldBe("a");
    }

    #endregion
}
