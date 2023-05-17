using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wilgysef.StdoutHook.Extensions;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public abstract class ProfileLoader
    {
        protected RuleLoader _ruleLoader = new RuleLoader();

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

        public Profile? LoadProfile(
            IReadOnlyList<ProfileDto> profileDtos,
            Func<IReadOnlyList<ProfileDto>, ProfileDto?> profileDtoPicker,
            bool throwIfInheritedProfileNotFound = true)
        {
            var graph = new ProfileDtoGraph(profileDtos.Count);
            var disabledProfileNames = new HashSet<string>();

            for (var i = 0; i < profileDtos.Count; i++)
            {
                var profile = profileDtos[i];
                if (profile.Enabled ?? true)
                {
                    graph.Add(profile);
                }
                else if (profile.ProfileName != null)
                {
                    disabledProfileNames.Add(profile.ProfileName);
                }
            }

            for (var i = 0; i < profileDtos.Count; i++)
            {
                var profile = profileDtos[i];
                if (!(profile.Enabled ?? true) || profile.InheritProfiles == null)
                {
                    continue;
                }

                for (var index = 0; index < profile.InheritProfiles.Count; index++)
                {
                    var name = profile.InheritProfiles[index];
                    if (!graph.Link(profile, name))
                    {
                        // TODO: log
                        if (throwIfInheritedProfileNotFound && !disabledProfileNames.Contains(name))
                        {
                            throw new InheritedProfileNotFoundException();
                        }
                    }
                }
            }

            var order = graph.GetTopologicalOrder();
            var loadedProfileDtos = new Dictionary<ProfileDto, ProfileDto>(order.Count);

            for (var i = order.Count - 1; i >= 0; i--)
            {
                var current = order[i];
                var names = current.InheritProfiles;
                var currentCopy = new ProfileDto
                {
                    Enabled = current.Enabled,
                    ProfileName = current.ProfileName
                };

                CombineProfileDto(currentCopy, current);

                if (names != null)
                {
                    for (var j = 0; j < names.Count; j++)
                    {
                        if (graph.TryGetProfileDtoByName(names[j], out var other))
                        {
                            loadedProfileDtos.TryGetValue(other, out var loadedOther);
                            CombineProfileDto(currentCopy, loadedOther);
                        }
                    }
                }

                loadedProfileDtos[current] = currentCopy;
            }

            var loadedProfileDtosOrdered = new List<ProfileDto>(order.Count);
            for (var i = 0; i < profileDtos.Count; i++)
            {
                var profile = profileDtos[i];
                if (profile.Enabled ?? true)
                {
                    loadedProfileDtosOrdered.Add(loadedProfileDtos[profile]);
                }
            }

            var profileDtoPicked = profileDtoPicker(loadedProfileDtosOrdered);
            return profileDtoPicked != null
                ? CreateProfile(profileDtoPicked)
                : null;
        }

        private List<Rule> LoadRules(IList<RuleDto>? ruleDtos)
        {
            if (ruleDtos == null || ruleDtos.Count == 0)
            {
                return new List<Rule>();
            }

            var rules = new List<Rule>(ruleDtos.Count);
            var rulesAdded = new HashSet<RuleDto>();

            for (var i = 0; i < ruleDtos.Count; i++)
            {
                var rule = ruleDtos[i];
                if (rule.Enabled.GetValueOrDefault(true) && rulesAdded.Add(rule))
                {
                    rules.Add(_ruleLoader.LoadRule(rule));
                }
            }

            return rules;
        }

        private Profile CreateProfile(ProfileDto dto)
        {
            return new Profile
            {
                ProfileName = dto.ProfileName,
                PseudoTty = dto.PseudoTty ?? false,
                Flush = dto.Flush ?? false,
                Rules = LoadRules(dto.Rules),
                CustomColors = dto.CustomColors ?? new Dictionary<string, string>(),
            };
        }

        private static void CombineProfileDto(ProfileDto target, ProfileDto source)
        {
            target.Command ??= source.Command;
            target.CommandExpression ??= source.CommandExpression;
            target.FullCommandPath ??= source.FullCommandPath;
            target.FullCommandPathExpression ??= source.FullCommandPathExpression;
            target.CommandIgnoreCase ??= source.CommandIgnoreCase;
            target.ArgumentPatterns ??= source.ArgumentPatterns;
            target.MinArguments ??= source.MinArguments;
            target.MaxArguments ??= source.MaxArguments;
            target.PseudoTty ??= source.PseudoTty;
            target.Flush ??= source.Flush;

            if (source.Rules != null)
            {
                target.Rules ??= new List<RuleDto>(source.Rules.Count);
                target.Rules.InsertRange(0, source.Rules);
            }

            if (source.CustomColors != null)
            {
                target.CustomColors ??= new Dictionary<string, string>();
                foreach (var (key, val) in source.CustomColors)
                {
                    target.CustomColors.TryAdd(key, val);
                }
            }
        }

        private class ProfileDtoGraph
        {
            private readonly Dictionary<ProfileDto, List<ProfileDto>> _adjacencyLists;
            private readonly Dictionary<string, ProfileDto> _profileDtos;

            public ProfileDtoGraph(int capacity)
            {
                _adjacencyLists = new Dictionary<ProfileDto, List<ProfileDto>>(capacity);
                _profileDtos = new Dictionary<string, ProfileDto>(capacity);
            }

            public void Add(ProfileDto profile)
            {
                _adjacencyLists[profile] = new List<ProfileDto>();

                if (profile.ProfileName != null)
                {
                    // only the first profile with the name is used
                    _profileDtos.TryAdd(profile.ProfileName, profile);
                }
            }

            public bool Link(ProfileDto profile, string profileName)
            {
                if (TryGetProfileDtoByName(profileName, out var other))
                {
                    _adjacencyLists[profile].Add(other);
                    return true;
                }

                return false;
            }

            public bool TryGetProfileDtoByName(string profileName, out ProfileDto profile)
            {
                return _profileDtos.TryGetValue(profileName, out profile);
            }

            public List<ProfileDto> GetTopologicalOrder()
            {
                var dtos = new List<ProfileDto>(_adjacencyLists.Count);
                var inDegrees = new Dictionary<ProfileDto, int>(_adjacencyLists.Count);

                foreach (var adjacents in _adjacencyLists.Values)
                {
                    for (var i = 0; i < adjacents.Count; i++)
                    {
                        var other = adjacents[i];
                        inDegrees.TryGetValue(other, out var count);
                        inDegrees[other] = count + 1;
                    }
                }

                var queue = new Queue<ProfileDto>();

                foreach (var profile in _adjacencyLists.Keys)
                {
                    if (!inDegrees.ContainsKey(profile))
                    {
                        queue.Enqueue(profile);
                    }
                }

                while (queue.Count > 0)
                {
                    var profile = queue.Dequeue();
                    dtos.Add(profile);

                    var adjacents = _adjacencyLists[profile];
                    for (var i = 0; i < adjacents.Count; i++)
                    {
                        var other = adjacents[i];
                        var count = inDegrees[other];
                        if (count > 1)
                        {
                            inDegrees[other] = count - 1;
                        }
                        else
                        {
                            queue.Enqueue(other);
                        }
                    }
                }

                if (dtos.Count < _adjacencyLists.Count)
                {
                    string? name = null;
                    foreach (var profile in _adjacencyLists.Keys)
                    {
                        if (!dtos.Contains(profile))
                        {
                            name = profile.ProfileName;
                            break;
                        }
                    }

                    throw new ProfileCyclicalInheritanceException(name ?? "");
                }

                return dtos;
            }
        }
    }
}
