using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Wilgysef.StdoutHook.Profiles.Dtos;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class ProfileDtoPicker
    {
        public ProfileDto? PickProfileDto(
            IReadOnlyList<ProfileDto> profileDtos,
            string? profileName = null,
            string? command = null,
            string? fullCommandPath = null,
            IReadOnlyList<string>? arguments = null)
        {
            for (var i = 0; i < profileDtos.Count; i++)
            {
                var dto = profileDtos[i];
                var stringComparison = (dto.CommandIgnoreCase ?? false)
                    ? StringComparison.CurrentCultureIgnoreCase
                    : StringComparison.CurrentCulture;

                if (!(dto.Enabled ?? true)
                    || !ArgumentsMatchPatterns(dto.ArgumentPatterns)
                    || !ArgumentsMatchSubcommand(dto.Subcommand, dto.SubcommandExpression, stringComparison))
                {
                    continue;
                }

                if (profileName != null && profileName == dto.ProfileName
                    || MatchesStringOrExpression(command, dto.Command, dto.CommandExpression, dto.CommandIgnoreCase ?? false)
                    || MatchesStringOrExpression(fullCommandPath, dto.FullCommandPath, dto.FullCommandPathExpression, dto.CommandIgnoreCase ?? false))
                {
                    return dto;
                }
            }

            return null;

            bool ArgumentsMatchSubcommand(object? subcommand, string? subcommandExpression, StringComparison stringComparison)
            {
                if (subcommand is string subcommandStr)
                {
                    if (arguments == null || arguments.Count == 0 || !arguments[0].Equals(subcommandStr, stringComparison))
                    {
                        return false;
                    }
                }
                else if (subcommand is IList<object> subcommandList)
                {
                    if (arguments == null || arguments.Count < subcommandList.Count)
                    {
                        return false;
                    }

                    for (var i = 0; i < subcommandList.Count; i++)
                    {
                        if (subcommandList[i] is string subcmd && !arguments[i].Equals(subcmd, stringComparison))
                        {
                            return false;
                        }
                    }
                }
                else if (subcommandExpression != null)
                {
                    if (arguments == null)
                    {
                        return false;
                    }

                    var builder = new StringBuilder();

                    for (var i = 0; i < arguments.Count && arguments[i][0] != '-'; i++)
                    {
                        builder.Append(arguments[i]);
                        builder.Append(' ');
                    }

                    var argSubcommands = builder.Remove(builder.Length - 1, 1)
                        .ToString();

                    if (!new Regex(subcommandExpression).IsMatch(argSubcommands))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool ArgumentsMatchPatterns(IList<ArgumentPatternDto>? argPatterns)
            {
                if (argPatterns == null)
                {
                    return true;
                }

                foreach (var argPattern in argPatterns)
                {
                    if (!(argPattern.Enabled ?? true))
                    {
                        continue;
                    }

                    if (argPattern.ArgumentExpression != null)
                    {
                        var mustNotMatch = argPattern.MustNotMatch ?? false;
                        if (arguments == null)
                        {
                            if (mustNotMatch)
                            {
                                continue;
                            }

                            return false;
                        }

                        var expression = new Regex(argPattern.ArgumentExpression, RegexOptions.Compiled);
                        var limit = Math.Min(argPattern.MaxPosition ?? arguments.Count, arguments.Count);

                        for (var argIndex = argPattern.MinPosition ?? 0; argIndex < limit; argIndex++)
                        {
                            if (expression.IsMatch(arguments[argIndex]) == mustNotMatch)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            static bool MatchesStringOrExpression(string? str, string? compare, string? expression, bool ignoreCase)
            {
                return str != null
                    && (str.Equals(compare, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture)
                        || (expression != null && new Regex(expression, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None).IsMatch(str)));
            }
        }
    }
}
