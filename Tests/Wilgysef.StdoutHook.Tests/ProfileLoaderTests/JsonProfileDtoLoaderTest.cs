using System.Text;
using Wilgysef.StdoutHook.Profiles;
using Wilgysef.StdoutHook.Profiles.Loaders;
using Wilgysef.StdoutHook.Rules;

namespace Wilgysef.StdoutHook.Tests.ProfileLoaderTests;

public class JsonProfileDtoLoaderTest
{
    [Fact]
    public async Task ConvertReplaceFieldsObjectToList()
    {
        var loader = new JsonProfileDtoLoader();
        var profile = await LoadProfileAsync(loader, GetStream(@"{
    ""Rules"": [
        {
            ""SeparatorExpression"": ""\\s+"",
            ""ReplaceFields"": [""a"", ""b""]
        }
    ]
}"));

        var replaceFields = ((FieldSeparatorRule)profile.Rules[0]).ReplaceFields!;
        replaceFields.Count.ShouldBe(2);
        replaceFields[0].Key.SingleValue!.Value.ShouldBe(1);
        replaceFields[0].Value.ShouldBe("a");
        replaceFields[1].Key.SingleValue!.Value.ShouldBe(2);
        replaceFields[1].Value.ShouldBe("b");
    }

    [Fact]
    public async Task ConvertReplaceFieldsObjectToDictionary()
    {
        var loader = new JsonProfileDtoLoader();
        var profile = await LoadProfileAsync(loader, GetStream(@"{
    ""Rules"": [
        {
            ""SeparatorExpression"": ""\\s+"",
            ""ReplaceFields"": {
                ""1-3"": ""a"",
                ""4"": ""b""
            }
        }
    ]
}"));

        var replaceFields = ((FieldSeparatorRule)profile.Rules[0]).ReplaceFields!;
        replaceFields.Count.ShouldBe(2);
        replaceFields[0].Key.GetMin().ShouldBe(1);
        replaceFields[0].Key.GetMax().ShouldBe(3);
        replaceFields[0].Value.ShouldBe("a");
        replaceFields[1].Key.SingleValue!.Value.ShouldBe(4);
        replaceFields[1].Value.ShouldBe("b");
    }

    [Fact]
    public async Task Invalid()
    {
        var loader = new JsonProfileDtoLoader();
        await Should.ThrowAsync<Exception>(() => LoadProfileAsync(loader, GetStream("test")));
    }

    private async Task<Profile> LoadProfileAsync(ProfileDtoLoader loader, Stream stream)
    {
        var profiles = await loader.LoadProfileDtosAsync(stream);
        return new ProfileLoader().LoadProfile(profiles, profileDtos => profileDtos[0])!;
    }

    private static Stream GetStream(string data)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(data));
    }
}
