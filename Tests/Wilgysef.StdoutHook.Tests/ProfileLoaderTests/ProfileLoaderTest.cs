using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class ProfileLoaderTest
{
    [Fact]
    public async Task ProfileName()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            ProfileName = "test",
        };

        using var profile = LoadProfile(loader, profileDto);
        profile.ProfileName.ShouldBe(profileDto.ProfileName);
    }

    [Fact]
    public async Task PseudoTty()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            PseudoTty = true,
        };

        using var profile = LoadProfile(loader, profileDto);
        profile.PseudoTty.ShouldBe(profileDto.PseudoTty.Value);
    }

    [Fact]
    public async Task Flush()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            Flush = true,
        };

        using var profile = LoadProfile(loader, profileDto);
        profile.Flush.ShouldBe(profileDto.Flush.Value);
    }

    [Fact]
    public async Task CustomColors()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            CustomColors = new Dictionary<string, string>
            {
                ["a"] = "b",
            },
        };

        using var profile = LoadProfile(loader, profileDto);
        profile.CustomColors.Count.ShouldBe(1);
        profile.CustomColors["a"].ShouldBe("b");
    }

    [Fact]
    public async Task ArgumentPattern()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            ArgumentPatterns = new[]
            {
                new ArgumentPatternDto
                {
                    ArgumentExpression = new List<object?> { "a[a-z]", "c" },
                },
            },
        };

        using var profile = LoadProfile(loader, profileDto);
    }

    [Fact]
    public async Task SkipDisabledRules()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            Rules = new List<RuleDto>
            {
                new RuleDto
                {
                    Enabled = false,
                    ReplaceAllFormat = "a",
                },
                new RuleDto
                {
                    ReplaceAllFormat = "a",
                },
            },
        };

        using var profile = LoadProfile(loader, profileDto);
        profile.Rules.Count.ShouldBe(1);
    }

    [Fact]
    public void ActivationExpressions()
    {
        var loader = new ProfileLoader();
        var profileDto = new ProfileDto
        {
            Rules = new List<RuleDto>
            {
                new RuleDto
                {
                    ReplaceAllFormat = "",
                    ActivationExpressions = new[] { new ActivationExpressionDto { Expression = "a" } },
                    ActivationExpressionsStdoutOnly = new[] { new ActivationExpressionDto { Expression = "a" } },
                    ActivationExpressionsStderrOnly = new[] { new ActivationExpressionDto { Expression = "a" } },
                    DeactivationExpressions = new[] { new ActivationExpressionDto { Expression = "a" } },
                    DeactivationExpressionsStdoutOnly = new[] { new ActivationExpressionDto { Expression = "a" } },
                    DeactivationExpressionsStderrOnly = new[] { new ActivationExpressionDto { Expression = "a" } },
                },
            },
        };

        using var profile = LoadProfile(loader, profileDto);
        profile!.Rules.Count.ShouldBe(1);
    }

    [Fact]
    public void InheritedProfile()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "inherited" },
            },
            new ProfileDto
            {
                ProfileName = "inherited",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test",
                    },
                }
            },
        };

        using var profile = loader.LoadProfile(
            profileDtos,
            profiles => profiles.Single(p => p.ProfileName == "testprofile"));

        profile!.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(1);
        ((UnconditionalReplaceRule)profile.Rules[0]).Format.ShouldBe("test");
    }

    [Fact]
    public void InheritedProfile_CopyProperties()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "inherited" },
                MinArguments = 3,
                CustomColors = new Dictionary<string, string>
                {
                    ["a"] = "1",
                },
            },
            new ProfileDto
            {
                ProfileName = "inherited",
                Command = "command",
                CommandExpression = "commandexpression",
                FullCommandPath = "fullcommandpath",
                FullCommandPathExpression = "fullcommandpathexpression",
                CommandIgnoreCase = true,
                ArgumentPatterns = new List<ArgumentPatternDto>
                {
                    new ArgumentPatternDto
                    {
                        ArgumentExpression = "argumentexpression",
                    }
                },
                MinArguments = 1,
                MaxArguments = 4,
                PseudoTty = true,
                Flush = true,
                CustomColors = new Dictionary<string, string>
                {
                    ["a"] = "b",
                    ["c"] = "d",
                },
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test",
                    },
                }
            },
        };

        using var profile = loader.LoadProfile(
            profileDtos,
            profiles =>
            {
                var selected = profiles.Single(p => p.ProfileName == "testprofile");

                selected.Command.ShouldBe("command");
                selected.GetCommandExpression().ShouldBe("commandexpression");
                selected.FullCommandPath.ShouldBe("fullcommandpath");
                selected.GetFullCommandPathExpression().ShouldBe("fullcommandpathexpression");
                selected.CommandIgnoreCase.ShouldBe(true);
                selected.ArgumentPatterns!.Count.ShouldBe(1);
                selected.ArgumentPatterns[0].GetArgumentExpression().ShouldBe("argumentexpression");
                selected.MinArguments.ShouldBe(3);
                selected.MaxArguments.ShouldBe(4);

                return selected;
            });

        profile!.ProfileName.ShouldBe("testprofile");
        profile.PseudoTty.ShouldBeTrue();
        profile.Flush.ShouldBeTrue();
        profile.CustomColors.Count.ShouldBe(2);
        profile.CustomColors["a"].ShouldBe("1");
        profile.CustomColors["c"].ShouldBe("d");
    }

    [Fact]
    public void InheritedProfile_NotEnabled()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "inherited" },
            },
            new ProfileDto
            {
                Enabled = false,
                ProfileName = "inherited",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test",
                    },
                }
            },
        };

        using var profile = loader.LoadProfile(
            profileDtos,
            profiles => profiles.Single(p => p.ProfileName == "testprofile"));

        profile!.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InheritedProfile_NotFound(bool throwException)
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "notfound" },
            },
        };

        if (throwException)
        {
            Should.Throw<InheritedProfileNotFoundException>(
                () => loader.LoadProfile(
                    profileDtos,
                    profiles => profiles.Single(p => p.ProfileName == "testprofile")));
        }
        else
        {
            using var profile = loader.LoadProfile(
                profileDtos,
                profiles => profiles.Single(p => p.ProfileName == "testprofile"),
                false);

            profile!.ProfileName.ShouldBe("testprofile");
        }
    }

    [Fact]
    public void InheritedProfile_Duplicate()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "inherited1", "inherited2" },
            },
            new ProfileDto
            {
                ProfileName = "inherited1",
                InheritProfiles = new List<string> { "inherited_sub" },
            },
            new ProfileDto
            {
                ProfileName = "inherited2",
                InheritProfiles = new List<string> { "inherited_sub" },
            },
            new ProfileDto
            {
                ProfileName = "inherited_sub",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test",
                    },
                },
            },
        };

        using var profile = loader.LoadProfile(
            profileDtos,
            profiles => profiles.Single(p => p.ProfileName == "testprofile"));

        profile!.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(1);
        ((UnconditionalReplaceRule)profile.Rules[0]).Format.ShouldBe("test");
    }

    [Fact]
    public void InheritedProfile_Cyclical()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "inherited" },
            },
            new ProfileDto
            {
                ProfileName = "inherited",
                InheritProfiles = new List<string> { "inherited_sub" },
            },
            new ProfileDto
            {
                ProfileName = "inherited_sub",
                InheritProfiles = new List<string> { "inherited" },
            },
        };

        Should.Throw<ProfileCyclicalInheritanceException>(
            () => loader.LoadProfile(
                profileDtos,
                profiles => profiles.Single(p => p.ProfileName == "testprofile")));
    }

    [Fact]
    public void InheritedProfile_MultipleRules()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test1",
                    },
                },
                InheritProfiles = new List<string> { "inherited" },
            },
            new ProfileDto
            {
                ProfileName = "inherited",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test2",
                    },
                },
                InheritProfiles = new List<string> { "inherited_sub" },
            },
            new ProfileDto
            {
                ProfileName = "inherited_sub",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test3",
                    },
                }
            },
        };

        using var profile = loader.LoadProfile(
            profileDtos,
            profiles => profiles.Single(p => p.ProfileName == "testprofile"));

        profile!.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(3);
        ((UnconditionalReplaceRule)profile.Rules[0]).Format.ShouldBe("test3");
        ((UnconditionalReplaceRule)profile.Rules[1]).Format.ShouldBe("test2");
        ((UnconditionalReplaceRule)profile.Rules[2]).Format.ShouldBe("test1");
    }

    [Fact]
    public void InheritedProfile_SelectedProfileMiddle()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "inherited",
                Rules = new List<RuleDto>
                {
                    new RuleDto
                    {
                        ReplaceAllFormat = "test",
                    },
                }
            },
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfiles = new List<string> { "inherited" },
            },
        };

        using var profile = loader.LoadProfile(
            profileDtos,
            profiles => profiles.Single(p => p.ProfileName == "testprofile"));

        profile!.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(1);
        ((UnconditionalReplaceRule)profile.Rules[0]).Format.ShouldBe("test");
    }

    [Fact]
    public void NoProfilePicked()
    {
        var loader = new ProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
            },
        };

        using var profile = loader.LoadProfile(profileDtos, profiles => null);

        profile.ShouldBeNull();
    }

    private static Profile LoadProfile(ProfileLoader loader, ProfileDto profileDto)
    {
        return loader.LoadProfile(new[] { profileDto }, profiles => profiles[0])!;
    }
}
