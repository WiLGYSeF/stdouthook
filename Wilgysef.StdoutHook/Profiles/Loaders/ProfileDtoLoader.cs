using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wilgysef.StdoutHook.Profiles.Dtos;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public abstract class ProfileDtoLoader
    {
        protected abstract Task<List<ProfileDto>> LoadProfileDtosInternalAsync(Stream stream, CancellationToken cancellationToken);

        public async Task<List<ProfileDto>> LoadProfileDtosAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var dtos = await LoadProfileDtosInternalAsync(stream, cancellationToken);

            for (var i = 0; i < dtos.Count; i++)
            {
                var dto = dtos[i];

                ThrowIfNotSplitExpression(dto.CommandExpression, nameof(dto.CommandExpression));
                ThrowIfNotSplitExpression(dto.FullCommandPathExpression, nameof(dto.FullCommandPathExpression));
                ThrowIfNotSplitExpression(dto.SubcommandExpression, nameof(dto.SubcommandExpression));

                if (dto.Subcommand != null
                    && !(dto.Subcommand is string)
                    && !IsStringList(dto.Subcommand))
                {
                    throw new InvalidPropertyTypeException(
                        nameof(dto.Subcommand),
                        dto.Subcommand.GetType().Name,
                        new[] { "string", "list of strings" });
                }

                if (dto.ArgumentPatterns != null)
                {
                    for (var argIndex = 0; argIndex < dto.ArgumentPatterns.Count; argIndex++)
                    {
                        var argPattern = dto.ArgumentPatterns[argIndex];
                        ThrowIfNotSplitExpression(argPattern.ArgumentExpression, nameof(argPattern.ArgumentExpression));
                    }
                }

                if (dto.Rules != null)
                {
                    for (var ruleIndex = 0; ruleIndex < dto.Rules.Count; ruleIndex++)
                    {
                        var rule = dto.Rules[ruleIndex];

                        ThrowIfNotSplitExpression(rule.EnableExpression, nameof(rule.EnableExpression));

                        ThrowIfActivationExpressionsNotSplitExpression(rule.ActivationExpressions);
                        ThrowIfActivationExpressionsNotSplitExpression(rule.ActivationExpressionsStdoutOnly);
                        ThrowIfActivationExpressionsNotSplitExpression(rule.ActivationExpressionsStderrOnly);
                        ThrowIfActivationExpressionsNotSplitExpression(rule.DeactivationExpressions);
                        ThrowIfActivationExpressionsNotSplitExpression(rule.DeactivationExpressionsStdoutOnly);
                        ThrowIfActivationExpressionsNotSplitExpression(rule.DeactivationExpressionsStderrOnly);

                        if (rule.ReplaceFields != null
                            && !(rule.ReplaceFields is IList<object?>)
                            && !(rule.ReplaceFields is IDictionary<string, object?>))
                        {
                            throw new InvalidPropertyTypeException(
                                nameof(rule.ReplaceFields),
                                rule.ReplaceFields.GetType().Name,
                                new[] { "list of strings", "object with string keys and string values" });
                        }

                        if (rule.ReplaceGroups != null
                            && !(rule.ReplaceGroups is IList<object?>)
                            && !(rule.ReplaceGroups is IDictionary<string, object?>))
                        {
                            throw new InvalidPropertyTypeException(
                                nameof(rule.ReplaceFields),
                                rule.ReplaceGroups.GetType().Name,
                                new[] { "list of strings", "object with string keys and string values" });
                        }
                    }
                }
            }

            return dtos;

            static void ThrowIfActivationExpressionsNotSplitExpression(IList<ActivationExpressionDto>? expressions)
            {
                if (expressions == null)
                {
                    return;
                }

                for (var i = 0; i < expressions.Count; i++)
                {
                    var expression = expressions[i];
                    ThrowIfNotSplitExpression(expression.Expression, nameof(expression.Expression));
                }
            }

            static void ThrowIfNotSplitExpression(object? value, string name)
            {
                if (value != null && !IsSplitExpressionType(value))
                {
                    throw new InvalidPropertyTypeException(name, value.GetType().Name, new[] { "string", "list of strings" });
                }
            }

            static bool IsSplitExpressionType(object? obj)
            {
                return obj is string || IsStringList(obj);
            }

            static bool IsStringList(object? obj)
            {
                if (!(obj is IList<object?> list))
                {
                    return false;
                }

                for (var i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is string))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
