﻿using Wilgysef.StdoutHook.Formatters;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.RuleTests;

public class UnconditionalReplaceRuleTest : RuleTestBase
{
    [Fact]
    public void UnconditionalRule()
    {
        var rule = new UnconditionalReplaceRule("asdf %test abc");

        ShouldRuleBe(
            rule,
            GetFormatter(new TestFormatBuilder("test", null, _ => "123")),
            "input",
            "asdf 123 abc");
    }

    private static void ShouldRuleBe(Rule rule, Formatter formatter, string input, string expected)
    {
        var state = new ProfileState();
        rule.Build(state, formatter);
        rule.Apply(new DataState(input, true, state)).ShouldBe(expected);
    }
}