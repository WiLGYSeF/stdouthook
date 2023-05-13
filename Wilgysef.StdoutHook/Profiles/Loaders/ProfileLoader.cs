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

        public abstract Task<ProfileDto> LoadProfileDtoAsync(Stream stream, CancellationToken cancellationToken = default);

        public Profile LoadProfile(IReadOnlyList<ProfileDto> profileDtos, ProfileDto profileToLoad)
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
                            stack.Push(node.AddChild(dtoByName));
                            count++;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }

            var traversal = new Stack<ProfileDto>(count);

            traversal.Push(root.ProfileDto);
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();

                for (var i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];
                    traversal.Push(child.ProfileDto);
                    stack.Push(child);
                }
            }

            var loaded = new HashSet<ProfileDto>(count);
            var currentDto = new ProfileDto();

            while (traversal.Count > 0)
            {
                var dto = traversal.Pop();
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
                rules.Add(_ruleLoader.LoadRule(ruleDtos[i]));
            }

            return rules;
        }

        private Profile CreateProfile(ProfileDto dto)
        {
            return new Profile
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
                CustomColors = dto.CustomColors ?? new Dictionary<string, string>(),
            };
        }

        private static void SetProfileDtoMetaProperties(ProfileDto target, ProfileDto source)
        {
            target.ProfileName = source.ProfileName;
            target.Command = source.Command;
            target.CommandExpression = source.CommandExpression;
            target.FullCommandPath = source.FullCommandPath;
            target.FullCommandPathExpression = source.FullCommandPathExpression;
            target.CommandIgnoreCase = source.CommandIgnoreCase;
            target.Enabled = source.Enabled;
        }

        private static void CombineProfileDto(ProfileDto target, ProfileDto source)
        {
            target.PseudoTty ??= source.PseudoTty;
            target.Flush ??= source.Flush;

            if (source.Rules != null)
            {
                target.Rules ??= new List<RuleDto>();
                for (var i = 0; i < source.Rules.Count; i++)
                {
                    target.Rules.Add(source.Rules[i]);
                }
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

        private static Regex? GetRegex(string? expression)
        {
            return expression != null
                ? new Regex(expression, RegexOptions.Compiled)
                : null;
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
                        throw new Exception();
                    }
                }

                var node = new ProfileDtoNode(profileDto, this);
                Children.Add(node);
                return node;
            }
        }
    }
}
