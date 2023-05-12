using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public abstract class ProfileLoader
    {
        protected RuleLoader _ruleLoader = new RuleLoader();

        protected abstract Task<ProfileDto> LoadProfileDtoAsync(Stream stream, CancellationToken cancellationToken);

        public async Task<Profile> LoadProfileAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var dto = await LoadProfileDtoAsync(stream, cancellationToken);

            var profile = new Profile
            {
                ProfileName = dto.ProfileName,
                Command = dto.Command,
                CommandExpression = GetRegex(dto.CommandExpression),
                FullCommandPath = dto.FullCommandPath,
                FullCommandPathExpression = GetRegex(dto.FullCommandPathExpression),
                CommandIgnoreCase = dto.CommandIgnoreCase.GetValueOrDefault(false),
                Enabled = dto.Enabled.GetValueOrDefault(true),
                PseudoTty = dto.PseudoTty.GetValueOrDefault(false),
                Flush = dto.Flush.GetValueOrDefault(false),
                Rules = LoadRules(dto.Rules),
                ColorAliases = dto.ColorAliases ?? new Dictionary<string, string>(),
                InheritProfileNames = dto.InheritProfileNames ?? new List<string>()
            };

            return profile;
        }

        private List<Rule> LoadRules(IList<RuleDto>? ruleDtos)
        {
            if (ruleDtos == null || ruleDtos.Count == 0)
            {
                return new List<Rule>();
            }

            var rules = new List<Rule>(ruleDtos.Count);

            for (var i = 0; i < ruleDtos.Count; i++)
            {
                rules.Add(_ruleLoader.LoadRule(ruleDtos[i]));
            }

            return rules;
        }

        private static Regex? GetRegex(string? expression)
        {
            return expression != null
                ? new Regex(expression, RegexOptions.Compiled)
                : null;
        }
    }
}
