namespace Bearz.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SerializationAttribute : Attribute
{
    public SerializationAttribute(string? name)
    {
        this.Name = name;
    }

    public string? Name { get; }

    public int Order { get; set; }

    public bool Ignore { get; set; }
}