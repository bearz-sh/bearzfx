namespace Bearz;

#pragma warning disable S2743 // requires a static properties.
#pragma warning disable S4035 // use IEquatable.
public class Enum<TEnum, TValue> : IEquatable<TEnum>, IComparable<TEnum>
    where TEnum : Enum<TEnum, TValue>
    where TValue : IComparable<TValue>
{
    public Enum(string name, TValue value)
    {
        this.Name = name;
        this.Value = value;
        var pos = Length;
        if (pos >= Set.Length)
        {
            var set = new TEnum[Set.Length * 2];
            Set.CopyTo(set, 0);
            Set = set;
        }

        Set[pos] = (TEnum)this;
        Length++;
    }

    public virtual string Label
    {
        get => this.Name;
    }

    public string Name { get; }

    public TValue Value { get; }

    private static TEnum[] Set { get; set;  } = new TEnum[10];

    private static int Length { get; set; } = 0;

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

    public static bool operator ==(Enum<TEnum, TValue>? left, Enum<TEnum, TValue>? right)
    {
        var lNull = ReferenceEquals(left, null);
        var rNull = ReferenceEquals(right, null);

        if (lNull && rNull)
            return true;

        if (!lNull && !rNull)
            return left!.Equals(right);

        return false;
    }

    public static bool operator !=(Enum<TEnum, TValue>? left, Enum<TEnum, TValue>? right)
    {
        var lNull = ReferenceEquals(left, null);
        var rNull = ReferenceEquals(right, null);

        if (lNull && rNull)
            return false;

        if (!lNull && !rNull)
            return !left!.Equals(right);

        return true;
    }

    public static bool operator ==(Enum<TEnum, TValue> left, TValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Enum<TEnum, TValue> left, TValue right)
    {
        return left.Equals(right);
    }

    public static bool operator >=(Enum<TEnum, TValue> left, Enum<TEnum, TValue> right)
    {
        return left.Value.CompareTo(right.Value) >= 0;
    }

    public static bool operator >=(Enum<TEnum, TValue> left, TValue right)
    {
        return left.Value.CompareTo(right) >= 0;
    }

    public static bool operator <=(Enum<TEnum, TValue> left, TValue right)
    {
        return left.Value.CompareTo(right) >= 0;
    }

    public static bool operator <=(Enum<TEnum, TValue> left, Enum<TEnum, TValue> right)
    {
        return left.Value.CompareTo(right.Value) <= 0;
    }

    public static bool operator >(Enum<TEnum, TValue> left, TValue right)
    {
        return left.Value.CompareTo(right) > 0;
    }

    public static bool operator <(Enum<TEnum, TValue> left, TValue right)
    {
        return left.Value.CompareTo(right) < 0;
    }

    public static bool operator >(Enum<TEnum, TValue> left, Enum<TEnum, TValue> right)
    {
        return left.Value.CompareTo(right.Value) > 0;
    }

    public static bool operator <(Enum<TEnum, TValue> left, Enum<TEnum, TValue> right)
    {
        return left.Value.CompareTo(right.Value) < 0;
    }

    public static IEnumerable<string> GetNames()
    {
        return Set.Take(Length).Select(x => x.Name);
    }

    public static IEnumerable<TValue> GetValues()
    {
        return Set.Take(Length).Select(x => x.Value);
    }

    public static TEnum[] GetValuesAsArray()
    {
        var copy = new TEnum[Length];
        Array.Copy(Set, copy, Length);
        return copy;
    }

    public static bool IsEnum(Type type)
    {
        if (type.IsEnum)
            return true;

        if (type == typeof(TEnum))
            return true;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Enum<,>))
            return true;

        var bt = type.BaseType;
        while (bt != null && bt != typeof(object))
        {
            if (bt.IsGenericType && bt.GetGenericTypeDefinition() == typeof(Enum<,>))
                return true;

            bt = bt.BaseType;
        }

        return false;
    }

    public static TEnum Parse(string name, bool caseInsensitive = false)
    {
        if (TryParse(name, caseInsensitive, out var value) && value != null)
            return value;

        throw new ArgumentException($"Invalid value '{name}' for enum '{typeof(TEnum).Name}'.");
    }

    public static bool TryParse(string name, out TEnum? value)
        => TryParse(name, false, out value);

    public static bool TryParse(string name, bool caseInsensitive, out TEnum? value)
    {
        var comparison = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        value = null;
        foreach (var item in Set)
        {
            if (item.Name.Equals(name, comparison) || item.Label.Equals(name, comparison))
            {
                value = item;
                return true;
            }
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Name, this.Label, this.Value);
    }

    public override string ToString()
    {
        return this.Name;
    }

    public int CompareTo(TEnum? other)
    {
        if (other is null)
            return -1;

        return this.Value.CompareTo(other.Value);
    }

    public virtual bool Equals(TEnum? other)
    {
        if (other is null)
            return false;

        return this.Name == other.Name && this.Value.CompareTo(other.Value) == 0;
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