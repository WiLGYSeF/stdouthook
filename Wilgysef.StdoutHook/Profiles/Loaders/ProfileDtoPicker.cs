using System.Collections.Generic;
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
                if (!dto.Enabled.GetValueOrDefault(true))
                {
                    continue;
                }

                if (profileName != null && profileName == dto.ProfileName
                    || command != null
                        && (command == dto.Command
                            || new Regex(dto.CommandExpression).Match(command).Success)
                    || fullCommandPath != null
                        && (fullCommandPath == dto.FullCommandPath
                            || new Regex(dto.FullCommandPathExpression).Match(fullCommandPath).Success))
                {
                    return dto;
                }
            }

            return null;
        }
    }
}
