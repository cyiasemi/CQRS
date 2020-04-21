
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.GetString().Length != 36)
            return Guid.ParseExact(reader.GetString(), "N");
        else
            return Guid.ParseExact(reader.GetString(), "D");
    }

    public override void Write(
        Utf8JsonWriter writer,
        Guid id,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(id.ToString(
                "D"));
}