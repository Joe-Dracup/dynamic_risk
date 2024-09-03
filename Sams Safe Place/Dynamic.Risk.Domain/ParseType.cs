using System.Text.Json.Serialization;
using System.Text.Json;

namespace Dynamic.Risk.Domain
{
    public enum ParseType
    {
        @string,
        number,
        definedListDetail,
        date
    }
    public class ParseTypeConverter : JsonConverter<ParseType>
    {
        public override ParseType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumString = reader.GetString();
                if (Enum.TryParse(typeof(ParseType), enumString, true, out var parsedValue))
                {
                    return (ParseType)parsedValue;
                }
            }

            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to {nameof(ParseType)}.");
        }

        public override void Write(Utf8JsonWriter writer, ParseType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLower());
        }
    }
}
