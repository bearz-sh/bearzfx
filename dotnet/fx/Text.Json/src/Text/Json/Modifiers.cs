using System.Reflection;
using System.Text.Json.Serialization.Metadata;

using Bearz.ComponentModel.DataAnnotations;
using Bearz.Extra.Strings;

namespace Bearz.Text.Json;

public static class Modifiers
{
    public static void CustomAttributes(JsonTypeInfo info)
    {
        foreach (var prop in info.Properties)
        {
            var propType = prop.PropertyType;
            var attr = propType.GetCustomAttribute<IgnoreAttribute>();
            if (attr is not null)
            {
                prop.ShouldSerialize = (_, _) => false;
                continue;
            }

            var serializationAttr = propType.GetCustomAttribute<SerializationAttribute>();
            if (serializationAttr is not null)
            {
                if (serializationAttr.Ignore)
                {
                    prop.ShouldSerialize = (_, _) => false;
                    continue;
                }

                if (!serializationAttr.Name.IsNullOrWhiteSpace())
                    prop.Name = serializationAttr.Name;

                if (prop.Order > 0)
                    prop.Order = serializationAttr.Order;
            }
        }
    }
}