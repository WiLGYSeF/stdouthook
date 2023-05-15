using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class ProfileLoaderTest
{
    [Fact]
    public async Task ProfileName()
    {
        var loader = new TestProfileLoader();
        loader.Profile.ProfileName = "test";

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.ProfileName.ShouldBe(loader.Profile.ProfileName);
    }

    [Fact]
    public async Task PseudoTty()
    {
        var loader = new TestProfileLoader();
        loader.Profile.PseudoTty = true;

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.PseudoTty.ShouldBe(loader.Profile.PseudoTty.Value);
    }

    [Fact]
    public async Task Flush()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Flush = true;

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.Flush.ShouldBe(loader.Profile.Flush.Value);
    }

    [Fact]
    public async Task CustomColors()
    {
        var loader = new TestProfileLoader();
        loader.Profile.CustomColors = new Dictionary<string, string>
        {
            ["a"] = "b",
        };

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.CustomColors.Count.ShouldBe(1);
        profile.CustomColors["a"].ShouldBe("b");
    }

    [Fact]
    public async Task SkipDisabledRules()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Rules = new List<RuleDto>
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
        };

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.Rules.Count.ShouldBe(1);
    }

    [Fact]
    public void InheritedProfile()
    {
        var loader = new TestProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfileNames = new List<string> { "inherited" },
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

        var profile = loader.LoadProfile(profileDtos, profileDtos[0]);
        profile.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(1);
        ((UnconditionalReplaceRule)profile.Rules[0]).Format.ShouldBe("test");
    }

    //[Fact]
    //public void InheritedProfile()
    //{
    //    var loader = new TestProfileLoader();
    //    var profileDtos = new[]
    //    {
    //        new ProfileDto
    //        {
    //            ProfileName = "testprofile",
    //            InheritProfileNames = new List<string> { "inherited" },
    //        },
    //        new ProfileDto
    //        {
    //            ProfileName = "inherited",
    //            Command = "donotuse",
    //            CommandExpression = "donotuse",
    //            FullCommandPath = "donotuse",
    //            FullCommandPathExpression = "donotuse",
    //            CommandIgnoreCase = true,
    //            ArgumentPatterns = new List<ArgumentPatternDto>
    //            {
    //                new ArgumentPatternDto
    //                {
    //                    ArgumentExpression = "donotuse",
    //                }
    //            },
    //            MinArguments = 1,
    //            MaxArguments = 2,
    //            PseudoTty = true,
    //            Flush = true,
    //            CustomColors = new Dictionary<string, string>
    //            {
    //                ["a"] = "b",
    //            },
    //            Rules = new List<RuleDto>
    //            {
    //                new RuleDto
    //                {
    //                    ReplaceAllFormat = "test",
    //                },
    //            }
    //        },
    //    };

    //    var profile = loader.LoadProfile(profileDtos, profileDtos[0]);
    //    profile.ProfileName.ShouldBe("testprofile");
    //}

    [Fact]
    public void InheritedProfile_NotEnabled()
    {
        var loader = new TestProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfileNames = new List<string> { "inherited" },
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

        var profile = loader.LoadProfile(profileDtos, profileDtos[0]);
        profile.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InheritedProfile_NotFound(bool throwException)
    {
        var loader = new TestProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfileNames = new List<string> { "notfound" },
            },
        };

        if (throwException)
        {
            Should.Throw<InheritedProfileNotFoundException>(() => loader.LoadProfile(profileDtos, profileDtos[0], throwException));
        }
        else
        {
            var profile = loader.LoadProfile(profileDtos, profileDtos[0], false);
            profile.ProfileName.ShouldBe("testprofile");
        }
    }

    [Fact]
    public void InheritedProfile_LoadDuplicate()
    {
        var loader = new TestProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfileNames = new List<string> { "inherited1", "inherited2" },
            },
            new ProfileDto
            {
                ProfileName = "inherited1",
                InheritProfileNames = new List<string> { "inherited_sub" },
            },
            new ProfileDto
            {
                ProfileName = "inherited2",
                InheritProfileNames = new List<string> { "inherited_sub" },
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

        var profile = loader.LoadProfile(profileDtos, profileDtos[0]);
        profile.ProfileName.ShouldBe("testprofile");
        profile.Rules.Count.ShouldBe(1);
        ((UnconditionalReplaceRule)profile.Rules[0]).Format.ShouldBe("test");
    }

    [Fact]
    public void InheritedProfile_Recursive()
    {
        var loader = new TestProfileLoader();
        var profileDtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "testprofile",
                InheritProfileNames = new List<string> { "inherited" },
            },
            new ProfileDto
            {
                ProfileName = "inherited",
                InheritProfileNames = new List<string> { "inherited_sub" },
            },
            new ProfileDto
            {
                ProfileName = "inherited_sub",
                InheritProfileNames = new List<string> { "inherited" },
            },
        };

        Should.Throw<ProfileInheritanceRecursionException>(() => loader.LoadProfile(profileDtos, profileDtos[0]));
    }

    [Fact]
    public async Task InvalidType_Subcommand()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Subcommand = false;

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtoAsync(null!));
    }

    [Fact]
    public async Task InvalidType_ReplaceFields()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Rules = new List<RuleDto>
        {
            new RuleDto
            {
                ReplaceFields = false,
            },
        };

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtoAsync(null!));
    }

    [Fact]
    public async Task InvalidType_ReplaceGroups()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Rules = new List<RuleDto>
        {
            new RuleDto
            {
                ReplaceGroups = false,
            },
        };

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtoAsync(null!));
    }

    private class TestProfileLoader : ProfileLoader
    {
        public ProfileDto Profile { get; set; } = new ProfileDto();

        protected override Task<ProfileDto> LoadProfileDtoInternalAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(Profile);
        }
    }
}
