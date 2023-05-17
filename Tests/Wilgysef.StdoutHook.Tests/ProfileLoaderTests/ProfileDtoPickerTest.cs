using Wilgysef.StdoutHook.Profiles.Dtos;
using Wilgysef.StdoutHook.Profiles.Loaders;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class ProfileDtoPickerTest
{
    [Fact]
    public void Enabled()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                Enabled = false,
                ProfileName = "test",
            },
            new ProfileDto
            {
                ProfileName = "test",
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBe(dtos[1]);
    }

    [Fact]
    public void ProfileName()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void Command()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                Command = "test",
            },
        };

        var dto = picker.PickProfileDto(dtos, command: "test");
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, command: "tEst");
        dto.ShouldBeNull();
    }

    [Fact]
    public void Command_IgnoreCase()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                Command = "test",
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, command: "test");
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, command: "tEst");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void CommandExpression()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                CommandExpression = "t[a-z]+t",
            },
        };

        var dto = picker.PickProfileDto(dtos, command: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void CommandExpression_IgnoreCase()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                CommandExpression = "t[a-z]+t",
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, command: "tEst");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void CommandExpression_Split()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                CommandExpression = new List<object?> { "t[a-z]", "+t" },
            },
        };

        var dto = picker.PickProfileDto(dtos, command: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void FullCommandPath()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                FullCommandPath = "test",
            },
        };

        var dto = picker.PickProfileDto(dtos, fullCommandPath: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void FullCommandPath_IgnoreCase()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                FullCommandPath = "test",
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, fullCommandPath: "tEst");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void FullCommandPathExpression()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                FullCommandPathExpression = "t[a-z]+t",
            },
        };

        var dto = picker.PickProfileDto(dtos, fullCommandPath: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void FullCommandPathExpression_IgnoreCase()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                FullCommandPathExpression = "t[a-z]+t",
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, fullCommandPath: "tEst");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void FullCommandPathExpression_Split()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                FullCommandPathExpression = new List<object?> { "t[a-z]", "+t" },
            },
        };

        var dto = picker.PickProfileDto(dtos, fullCommandPath: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void Subcommand_String()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                Subcommand = "abc",
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "nomatch" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: Array.Empty<string>());
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBeNull();
    }

    [Fact]
    public void Subcommand_List()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                Subcommand = new[] { "abc", "def" },
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "nomatch" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBeNull();
    }

    [Fact]
    public void Subcommand_Expression()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                SubcommandExpression = "a[a-z]c",
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void Subcommand_Expression_Multiple()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                SubcommandExpression = "a[a-z]c def",
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBeNull();
    }

    [Fact]
    public void Subcommand_Expression_Split()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                SubcommandExpression = new List<object?> { "abc", "def" },
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abcdef" });
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void ArgumentPattern()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                ArgumentPatterns = new[]
                {
                    new ArgumentPatternDto
                    {
                        ArgumentExpression = "a[a-z]c",
                    },
                },
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "def" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBeNull();
    }

    [Fact]
    public void ArgumentPattern_Range()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                ArgumentPatterns = new[]
                {
                    new ArgumentPatternDto
                    {
                        ArgumentExpression = "a[a-z]c",
                        MinPosition = 2,
                        MaxPosition = 3,
                    },
                },
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "z", "abc" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "z", "z", "abc" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "z", "z", "z", "abc" });
        dto.ShouldBeNull();
    }

    [Fact]
    public void ArgumentPattern_MustNotMatch()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                ArgumentPatterns = new[]
                {
                    new ArgumentPatternDto
                    {
                        ArgumentExpression = "a[a-z]c",
                        MinPosition = 2,
                        MaxPosition = 3,
                        MustNotMatch = true,
                    },
                },
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "z", "abc" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "z", "z", "abc" });
        dto.ShouldBeNull();

        dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "z", "z", "z", "abc" });
        dto.ShouldBe(dtos[0]);

        dto = picker.PickProfileDto(dtos, profileName: "test");
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void ArgumentPattern_NoExpression()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                ArgumentPatterns = new[]
                {
                    new ArgumentPatternDto(),
                },
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void ArgumentPattern_NotEnabled()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                ArgumentPatterns = new[]
                {
                    new ArgumentPatternDto
                    {
                        Enabled = false,
                        ArgumentExpression = "a[a-z]c",
                    },
                },
                CommandIgnoreCase = true,
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "def" });
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void ArgumentPattern_Expression_Split()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
                ArgumentPatterns = new[]
                {
                    new ArgumentPatternDto
                    {
                        ArgumentExpression = new List<object?> { "a[a-z]", "c" },
                    },
                },
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "test", arguments: new[] { "abc", "def" });
        dto.ShouldBe(dtos[0]);
    }

    [Fact]
    public void Match_None()
    {
        var picker = new ProfileDtoPicker();
        var dtos = new[]
        {
            new ProfileDto
            {
                ProfileName = "test",
            },
        };

        var dto = picker.PickProfileDto(dtos, profileName: "notexist");
        dto.ShouldBeNull();
    }
}
