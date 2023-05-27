using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class ProfileDtoLoaderTest
{
    [Fact]
    public async Task InvalidType_CommandExpression()
    {
        var loader = new TestProfileLoader();

        loader.Profile.CommandExpression = false;
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));

        loader.Profile.CommandExpression = new List<object?> { "a", 1 };
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    [Fact]
    public async Task InvalidType_FullCommandPathExpression()
    {
        var loader = new TestProfileLoader();

        loader.Profile.FullCommandPathExpression = false;
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));

        loader.Profile.FullCommandPathExpression = new List<object?> { "a", 1 };
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    [Fact]
    public async Task InvalidType_Subcommand()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Subcommand = false;

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    [Fact]
    public async Task InvalidType_SubcommandExpression()
    {
        var loader = new TestProfileLoader();

        loader.Profile.SubcommandExpression = false;
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));

        loader.Profile.SubcommandExpression = new List<object?> { "a", 1 };
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    [Fact]
    public async Task InvalidType_ArgumentPatterns()
    {
        var loader = new TestProfileLoader();

        loader.Profile.ArgumentPatterns = new[]
        {
            new ArgumentPatternDto
            {
                ArgumentExpression = false,
            },
        };
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));

        loader.Profile.ArgumentPatterns = new[]
        {
            new ArgumentPatternDto
            {
                ArgumentExpression = new List<object?> { "a", 1 },
            },
        };
        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    [Fact]
    public async Task InvalidType_EnableExpression()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Rules = new List<RuleDto>
        {
            new RuleDto
            {
                EnableExpression = false,
            },
        };

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    [Fact]
    public async Task InvalidType_ActivationExpression()
    {
        var loader = new TestProfileLoader();
        loader.Profile.Rules = new List<RuleDto>
        {
            new RuleDto
            {
                ActivationExpressions = new[]
                {
                    new ActivationExpressionDto
                    {
                        Expression = false,
                    },
                }
            },
        };

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
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

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
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

        await Should.ThrowAsync<InvalidPropertyTypeException>(() => loader.LoadProfileDtosAsync(null!));
    }

    private class TestProfileLoader : ProfileDtoLoader
    {
        public ProfileDto Profile { get; set; } = new ProfileDto();

        protected override Task<List<ProfileDto>> LoadProfileDtosInternalAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<ProfileDto> { Profile });
        }
    }
}
