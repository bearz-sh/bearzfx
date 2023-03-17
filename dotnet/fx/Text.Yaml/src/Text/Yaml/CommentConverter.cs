using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Bearz.Text.Yaml;

public class CommentConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(Comment);
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        var comment = new Comment();
        while (parser.Current is YamlDotNet.Core.Events.Comment commentEvent)
        {
            comment.Add(commentEvent.Value);
            parser.MoveNext();
        }

        return comment;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value is Comment comment)
        {
            foreach (var line in comment)
            {
                emitter.Emit(new YamlDotNet.Core.Events.Comment(line, true));
            }
        }
    }
}