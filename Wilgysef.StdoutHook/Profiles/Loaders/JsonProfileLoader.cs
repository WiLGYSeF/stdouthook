using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Wilgysef.StdoutHook.Profiles.Dtos;

namespace Wilgysef.StdoutHook.Profiles.Loaders
{
    public class JsonProfileLoader : ProfileLoader
    {
        protected override async Task<ProfileDto> LoadProfileDtoInternalAsync(Stream stream, CancellationToken cancellationToken)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new ObjectConverter());

            var dto = await JsonSerializer.DeserializeAsync<ProfileDto>(stream, options, cancellationToken);

            if (dto == null)
            {
                throw new Exception();
            }

            return dto;
        }

        private class ObjectConverter : JsonConverter<object>
        {
            private readonly Dictionary<JsonSerializerOptions, JsonConverter<JsonElement>> _converters = new Dictionary<JsonSerializerOptions, JsonConverter<JsonElement>>();

            public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (!_converters.TryGetValue(options, out var converter))
                {
                    converter = (JsonConverter<JsonElement>)options.GetConverter(typeof(JsonElement));
                    _converters[options] = converter;
                }

                var element = converter.Read(ref reader, typeToConvert, options);
                return GetJsonElementValue(ref element);
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            private static object? GetJsonElementValue(ref JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        var count = element.GetArrayLength();
                        var list = new List<object?>(count);

                        foreach (var item in element.EnumerateArray())
                        {
                            var current = item;
                            list.Add(GetJsonElementValue(ref current));
                        }

                        return list;
                    case JsonValueKind.False:
                        return false;
                    case JsonValueKind.Null:
                        return null;
                    case JsonValueKind.Number:
                        if (element.TryGetInt32(out var int32))
                        {
                            return int32;
                        }
                        else if (element.TryGetInt64(out var int64))
                        {
                            return int64;
                        }
                        else if (element.TryGetDouble(out var doubleVal))
                        {
                            return doubleVal;
                        }

                        return element;
                    case JsonValueKind.Object:
                        var obj = new Dictionary<string, object?>();

                        foreach (var prop in element.EnumerateObject())
                        {
                            var value = prop.Value;
                            obj[prop.Name] = GetJsonElementValue(ref value);
                        }

                        return obj;
                    case JsonValueKind.String:
                        return element.GetString();
                    case JsonValueKind.True:
                        return true;
                    case JsonValueKind.Undefined:
                        return null;
                    default:
                        return element;
                }
            }
        }
    }
}
