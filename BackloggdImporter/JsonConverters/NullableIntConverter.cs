using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BackloggdImporter.JsonConverters;

internal class NullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                var stringValue = reader.GetString();
                if (int.TryParse(stringValue, out var intValue))
                {
                    return intValue;
                }

                return null;
            }
            case JsonTokenType.Number:
                return reader.GetInt32();
            case JsonTokenType.Null:
                return null;
            default:
                throw new JsonException($"Cannot convert {reader.TokenType} to int?");
        }
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}