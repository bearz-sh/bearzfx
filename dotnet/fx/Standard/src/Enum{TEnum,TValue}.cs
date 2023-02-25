namespace Bearz;

#pragma warning disable S2743 // requires a static properties.
#pragma warning disable S4035 // use IEquatable.
public class Enum<TEnum, TValue> : IEquatable<TEnum>
    where TEnum : Enum<TEnum, TValue>
{
    public Enum(string name, TValue value)
    {
        this.Name = name;
        this.Value = value;
        var set = new TEnum[Set.Length + 1];
        Set.CopyTo(set, 0);
        set[Set.Length] = (TEnum)this;
    }

    public static IEnumerable<string> Names
    {
        get
        {
            return Set.Select(x => x.Name);
        }
    }

    public static IEnumerable<TValue> Values
    {
        get
        {
            return Set.Select(x => x.Value);
        }
    }

    public string Name { get; }

    public TValue Value { get; }

    private static TEnum[] Set { get; } = Array.Empty<TEnum>();

    public static implicit operator Enum<TEnum, TValue>?(string name)
    {
        TryParse(name, out var value);
        return value;
    }

    public static implicit operator string(Enum<TEnum, TValue> value)
    {
        return value.Name;
    }

    public static implicit operator TValue(Enum<TEnum, TValue> value)
    {
        return value.Value;
    }

    public static bool TryParse(string name, out TEnum? value)
        => TryParse(name, false, out value);

    public static bool TryParse(string name, bool caseInsensitive, out TEnum? value)
    {
        var comparison = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        value = null;
        foreach (var item in Set)
        {
            if (item.Name.Equals(name, comparison))
            {
                value = item;
                return true;
            }
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Name, this.Value);
    }

    public override string ToString()
    {
        return this.Name;
    }

    public bool Equals(TEnum? other)
    {
        if (other is null)
            return false;

        return this.Name == other.Name && EqualityComparer<TValue>.Default.Equals(this.Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is TEnum other)
            return this.Equals(other);

        return false;
    }
}