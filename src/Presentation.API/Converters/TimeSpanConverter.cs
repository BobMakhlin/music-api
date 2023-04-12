using System.Text.Json;
using System.Text.Json.Serialization;

namespace Presentation.API.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("minutes", value.Minutes);
            writer.WriteNumber("seconds", value.Seconds);
            writer.WriteEndObject();                    
        }
    }
}
