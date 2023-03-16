using System.Text.Json;

namespace Bearz.Text.Json;

public class CommentConverter : System.Text.Json.Serialization.JsonConverter<Comment>
{
    public override Comment? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Comment)
        {
            return new Comment(reader.GetComment());
        }

        return default!;
    }

    public override void Write(Utf8JsonWriter writer, Comment value, JsonSerializerOptions options)
    {
        writer.WriteCommentValue(value.ToString());
    }
}