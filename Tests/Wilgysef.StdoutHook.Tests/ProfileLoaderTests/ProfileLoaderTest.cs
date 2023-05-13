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
    public async Task Command()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Command = "test";

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.Command.ShouldBe(loader.Profile.Command);
    }

    [Fact]
    public async Task CommandExpression()
    {
        var loader = new TestProfileLoader();
        loader.Profile.CommandExpression = "test";

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.CommandExpression!.ToString().ShouldBe(loader.Profile.CommandExpression);
    }

    [Fact]
    public async Task FullCommandPath()
    {
        var loader = new TestProfileLoader();
        loader.Profile.FullCommandPath = "test";

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.FullCommandPath.ShouldBe(loader.Profile.FullCommandPath);
    }

    [Fact]
    public async Task FullCommandPathExpression()
    {
        var loader = new TestProfileLoader();
        loader.Profile.FullCommandPathExpression = "test";

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.FullCommandPathExpression!.ToString().ShouldBe(loader.Profile.FullCommandPathExpression);
    }

    [Fact]
    public async Task CommandIgnoreCase()
    {
        var loader = new TestProfileLoader();
        loader.Profile.CommandIgnoreCase = true;

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.CommandIgnoreCase.ShouldBe(loader.Profile.CommandIgnoreCase.Value);
    }

    [Fact]
    public async Task Enabled()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Enabled = false;

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.Enabled.ShouldBe(loader.Profile.Enabled.Value);
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
    public async Task InheritProfileNames()
    {
        var loader = new TestProfileLoader();
        loader.Profile.InheritProfileNames = new List<string> { "a" };

        var profile = await loader.LoadProfileAsync(new MemoryStream());
        profile.InheritProfileNames.Count.ShouldBe(1);
        profile.InheritProfileNames[0].ShouldBe(loader.Profile.InheritProfileNames[0]);
    }

    private class TestProfileLoader : ProfileLoader
    {
        public ProfileDto Profile { get; set; } = new ProfileDto();

        protected override Task<ProfileDto> LoadProfileDtoAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(Profile);
        }
    }
}
