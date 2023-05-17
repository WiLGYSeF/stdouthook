using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class RuleLoader
    {
        private static readonly Dictionary<Type, Func<RuleDto, Rule>> RuleBuilders = new Dictionary<Type, Func<RuleDto, Rule>>
        {
            [typeof(FieldSeparatorRule)] = BuildFieldSeparatorRule,
            [typeof(FilterRule)] = BuildFilterRule,
            [typeof(RegexGroupRule)] = BuildRegexGroupRule,
            [typeof(TeeRule)] = BuildTeeRule,
            [typeof(UnconditionalReplaceRule)] = BuildUnconditionalReplaceRule,
        };

        public Rule LoadRule(RuleDto dto)
        {
            return RuleBuilders.TryGetValue(GetRuleType(dto), out var builder)
                ? builder(dto)
                : throw new UnknownRuleException();
        }

        private static Rule BuildFieldSeparatorRule(RuleDto dto)
        {
            var rule = new FieldSeparatorRule(CreateRegex(dto.GetSeparatorExpression()!));
            SetBaseRuleProperties(rule, dto);

            rule.MinFields = dto.MinFields;
            rule.MaxFields = dto.MaxFields;

            if (dto.ReplaceAllFormat != null)
            {
                rule.ReplaceAllFormat = dto.ReplaceAllFormat;
            }
            else if (dto.ReplaceFields != null)
            {
                rule.ReplaceFields = BuildReplaceFields(rule, dto.ReplaceFields, null);
            }
            else
            {
                throw new InvalidRuleException(rule, $"missing {nameof(dto.ReplaceAllFormat)} or {nameof(dto.ReplaceFields)}");
            }

            return rule;
        }

        private static Rule BuildFilterRule(RuleDto dto)
        {
            var rule = new FilterRule();
            SetBaseRuleProperties(rule, dto);
            return rule;
        }

        private static Rule BuildRegexGroupRule(RuleDto dto)
        {
            var rule = new RegexGroupRule(CreateRegex(dto.GetRegex()!));
            SetBaseRuleProperties(rule, dto);

            if (dto.ReplaceAllFormat != null)
            {
                rule.ReplaceAllFormat = dto.ReplaceAllFormat;
            }
            else if (dto.ReplaceGroups != null)
            {
                var otherKeys = new List<KeyValuePair<string, string>>();

                rule.ReplaceGroups = BuildReplaceFields(rule, dto.ReplaceGroups, otherKeys);

                if (otherKeys.Count > 0)
                {
                    rule.ReplaceNamedGroups = new Dictionary<string, string>();
                    foreach (var (key, val) in otherKeys)
                    {
                        rule.ReplaceNamedGroups[key] = val;
                    }
                }
            }
            else
            {
                throw new InvalidRuleException(rule, $"missing {nameof(dto.ReplaceAllFormat)} or {nameof(dto.ReplaceGroups)}");
            }

            return rule;
        }

        private static List<KeyValuePair<FieldRangeList, string>> BuildReplaceFields(
            Rule rule,
            object replace,
            List<KeyValuePair<string, string>>? otherObjectKeys)
        {
            if (replace is IList<object?> fieldsList)
            {
                var replaceFields = new List<KeyValuePair<FieldRangeList, string>>(fieldsList.Count);

                for (var i = 0; i < fieldsList.Count; i++)
                {
                    if (!(fieldsList[i] is string str))
                    {
                        throw new InvalidRuleException(rule, "expected string type");
                    }

                    replaceFields.Add(new KeyValuePair<FieldRangeList, string>(
                        new FieldRangeList(new FieldRange(i + 1)),
                        str));
                }

                return replaceFields;
            }
            else if (replace is IDictionary<string, object?> fieldsObj)
            {
                var replaceFields = new List<KeyValuePair<FieldRangeList, string>>(fieldsObj.Count);

                foreach (var (key, val) in fieldsObj)
                {
                    if (!(val is string str))
                    {
                        throw new InvalidRuleException(rule, "expected string type");
                    }

                    if (FieldRangeList.TryParse(key, out var rangeList))
                    {
                        replaceFields.Add(new KeyValuePair<FieldRangeList, string>(
                            rangeList,
                            str));
                    }
                    else
                    {
                        otherObjectKeys?.Add(new KeyValuePair<string, string>(key, str));
                    }
                }

                return replaceFields;
            }

            throw new InvalidRuleException(rule, "expected list or object type");
        }

        private static Rule BuildTeeRule(RuleDto dto)
        {
            var rule = new TeeRule(dto.Filename!);
            SetBaseRuleProperties(rule, dto);

            rule.Flush = dto.Flush ?? false;
            rule.ExtractColors = dto.ExtractColors ?? false;

            return rule;
        }

        private static Rule BuildUnconditionalReplaceRule(RuleDto dto)
        {
            var rule = new UnconditionalReplaceRule(dto.ReplaceAllFormat!);
            SetBaseRuleProperties(rule, dto);

            return rule;
        }

        private static void SetBaseRuleProperties(Rule rule, RuleDto dto)
        {
            rule.EnableExpression = GetRegex(dto.GetEnableExpression());
            rule.StdoutOnly = dto.StdoutOnly ?? false;
            rule.StderrOnly = dto.StderrOnly ?? false;
            rule.Terminal = dto.Terminal ?? false;
            rule.TrimNewline = dto.TrimNewline ?? false;
            rule.ActivationLines = dto.ActivationLines ?? new List<long>();
            rule.ActivationLinesStdoutOnly = dto.ActivationLinesStdoutOnly ?? new List<long>();
            rule.ActivationLinesStderrOnly = dto.ActivationLinesStderrOnly ?? new List<long>();
            rule.DeactivationLines = dto.DeactivationLines ?? new List<long>();
            rule.DeactivationLinesStdoutOnly = dto.DeactivationLinesStdoutOnly ?? new List<long>();
            rule.DeactivationLinesStderrOnly = dto.DeactivationLinesStderrOnly ?? new List<long>();
            rule.ActivationExpressions = CreateActivationExpression(dto.ActivationExpressions);
            rule.ActivationExpressionsStdoutOnly = CreateActivationExpression(dto.ActivationExpressionsStdoutOnly);
            rule.ActivationExpressionsStderrOnly = CreateActivationExpression(dto.ActivationExpressionsStderrOnly);
            rule.DeactivationExpressions = CreateActivationExpression(dto.DeactivationExpressions);
            rule.DeactivationExpressionsStdoutOnly = CreateActivationExpression(dto.DeactivationExpressionsStdoutOnly);
            rule.DeactivationExpressionsStderrOnly = CreateActivationExpression(dto.DeactivationExpressionsStderrOnly);
        }

        private static List<ActivationExpression> CreateActivationExpression(IList<ActivationExpressionDto>? dtos)
        {
            if (dtos == null)
            {
                return new List<ActivationExpression>();
            }

            var expressions = new List<ActivationExpression>(dtos.Count);

            for (var i = 0; i < dtos.Count; i++)
            {
                var expression = CreateActivationExpression(dtos[i]);
                if (expression != null)
                {
                    expressions.Add(expression);
                }
            }

            return expressions;
        }

        private static ActivationExpression? CreateActivationExpression(ActivationExpressionDto dto)
        {
            if (dto.Expression == null)
            {
                return null;
            }

            return new ActivationExpression(
                CreateRegex(dto.Expression),
                dto.ActivationOffset ?? 0);
        }

        private static Type GetRuleType(RuleDto rule)
        {
            if (rule.Filter.HasValue && rule.Filter.Value)
            {
                return typeof(FilterRule);
            }

            Type? type = null;

            if (rule.SeparatorExpression != null)
            {
                SetType(ref type, typeof(FieldSeparatorRule));
            }

            if (rule.Regex != null)
            {
                SetType(ref type, typeof(RegexGroupRule));
            }

            if (rule.Filename != null)
            {
                SetType(ref type, typeof(TeeRule));
            }

            if (type == null && rule.ReplaceAllFormat != null)
            {
                SetType(ref type, typeof(UnconditionalReplaceRule));
            }

            return type ?? throw new UnknownRuleException();

            static void SetType(ref Type? type, Type value)
            {
                if (type != null)
                {
                    throw new UnknownRuleException();
                }

                type = value;
            }
        }

        private static Regex? GetRegex(string? expression)
        {
            return expression != null
                ? CreateRegex(expression)
                : null;
        }

        private static Regex CreateRegex(string expression) => new Regex(expression, RegexOptions.Compiled);
    }
}
