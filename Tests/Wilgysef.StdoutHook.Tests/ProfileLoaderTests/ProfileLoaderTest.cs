using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

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

    private class TestProfileLoader : ProfileLoader
    {
        public ProfileDto Profile { get; set; } = new ProfileDto();

        protected override Task<ProfileDto> LoadProfileDtoInternalAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(Profile);
        }
    }
}
