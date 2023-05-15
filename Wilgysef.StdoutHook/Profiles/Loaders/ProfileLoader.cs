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

        protected abstract Task<ProfileDto> LoadProfileDtoInternalAsync(Stream stream, CancellationToken cancellationToken);

        public async Task<ProfileDto> LoadProfileDtoAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var dto = await LoadProfileDtoInternalAsync(stream, cancellationToken);

            if (dto.Subcommand != null
                && !(dto.Subcommand is string)
                && !(dto.Subcommand is IList<object>))
            {
                throw new InvalidPropertyTypeException(
                    nameof(dto.Subcommand),
                    dto.Subcommand.GetType().Name,
                    new[] { "string", "list of strings" });
            }

            if (dto.Rules != null)
            {
                for (var ruleIndex = 0; ruleIndex < dto.Rules.Count; ruleIndex++)
                {
                    var rule = dto.Rules[ruleIndex];
                    if (rule.ReplaceFields != null
                        && !(rule.ReplaceFields is IList<object>)
                        && !(rule.ReplaceFields is IDictionary<string, object>))
                    {
                        throw new InvalidPropertyTypeException(
                            nameof(rule.ReplaceFields),
                            rule.ReplaceFields.GetType().Name,
                            new[] { "list of strings", "object with string keys and string values" });
                    }

                    if (rule.ReplaceGroups != null
                        && !(rule.ReplaceGroups is IList<object>)
                        && !(rule.ReplaceGroups is IDictionary<string, object>))
                    {
                        throw new InvalidPropertyTypeException(
                            nameof(rule.ReplaceFields),
                            rule.ReplaceGroups.GetType().Name,
                            new[] { "list of strings", "object with string keys and string values" });
                    }
                }
            }

            return dto;
        }

        public Profile LoadProfile(
            IReadOnlyList<ProfileDto> profileDtos,
            ProfileDto profileToLoad,
            bool throwIfInheritedProfileNotFound = true)
        {
            var profileDtoMap = new Dictionary<string, ProfileDto>();

            for (var i = 0; i < profileDtos.Count; i++)
            {
                var profile = profileDtos[i];
                if (profile.ProfileName != null)
                {
                    profileDtoMap.TryAdd(profile.ProfileName, profile);
                }
            }

            var root = new ProfileDtoNode(profileToLoad, null);

            var stack = new Stack<ProfileDtoNode>();
            stack.Push(root);

            var count = 0;

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                var dto = node.ProfileDto;

                if (dto.InheritProfileNames != null)
                {
                    for (var i = 0; i < dto.InheritProfileNames.Count; i++)
                    {
                        if (profileDtoMap.TryGetValue(dto.InheritProfileNames[i], out var dtoByName))
                        {
                            if (dtoByName.Enabled.GetValueOrDefault(true))
                            {
                                stack.Push(node.AddChild(dtoByName));
                                count++;
                            }
                        }
                        else
                        {
                            // TODO: log
                            if (throwIfInheritedProfileNotFound)
                            {
                                throw new InheritedProfileNotFoundException();
                            }
                        }
                    }
                }
            }

            var traversal = new List<ProfileDto>(count)
            {
                root.ProfileDto
            };
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                for (var i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];
                    traversal.Add(child.ProfileDto);
                    stack.Push(child);
                }
            }

            var loaded = new HashSet<ProfileDto>(count);
            var currentDto = new ProfileDto();

            for (var i = 0; i < traversal.Count; i++)
            {
                var dto = traversal[i];
                if (!loaded.Add(dto))
                {
                    continue;
                }

                CombineProfileDto(currentDto, dto);
            }

            SetProfileDtoMetaProperties(currentDto, profileToLoad);
            return CreateProfile(currentDto);
        }

        public async Task<Profile> LoadProfileAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var dto = await LoadProfileDtoAsync(stream, cancellationToken);
            return CreateProfile(dto);
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
                var rule = ruleDtos[i];
                if (rule.Enabled.GetValueOrDefault(true))
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

        private static void SetProfileDtoMetaProperties(ProfileDto target, ProfileDto source)
        {
            target.Enabled ??= source.Enabled;
            target.ProfileName ??= source.ProfileName;
            target.Command ??= source.Command;
            target.CommandExpression ??= source.CommandExpression;
            target.FullCommandPath ??= source.FullCommandPath;
            target.FullCommandPathExpression ??= source.FullCommandPathExpression;
            target.CommandIgnoreCase ??= source.CommandIgnoreCase;
            target.ArgumentPatterns ??= source.ArgumentPatterns;
            target.MinArguments ??= source.MinArguments;
            target.MaxArguments ??= source.MaxArguments;
        }

        private static void CombineProfileDto(ProfileDto target, ProfileDto source)
        {
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

        private class ProfileDtoNode
        {
            public ProfileDto ProfileDto { get; }

            public ProfileDtoNode? Parent { get; }

            public List<ProfileDtoNode> Children { get; } = new List<ProfileDtoNode>();

            public ProfileDtoNode(ProfileDto profileDto, ProfileDtoNode? parent)
            {
                ProfileDto = profileDto;
                Parent = parent;
            }

            public ProfileDtoNode AddChild(ProfileDto profileDto)
            {
                for (var current = this; current != null; current = current.Parent)
                {
                    if (current.ProfileDto == profileDto)
                    {
                        throw new ProfileInheritanceRecursionException(profileDto.ProfileName!);
                    }
                }

                var node = new ProfileDtoNode(profileDto, this);
                Children.Add(node);
                return node;
            }
        }
    }
}
