namespace Bearz.Diagnostics;
public class CollectionCapture : IProcessCapture
{
    private readonly ICollection<string> collection;

    public CollectionCapture()
        : this(new List<string>())
    {
    }

    public CollectionCapture(ICollection<string> collection)
    {
        this.collection = collection;
    }

    public void WriteLine(string value)
    {
        this.collection.Add(value);
    }

    public IReadOnlyList<string> ToReadOnlyList()
    {
        if (this.collection is IReadOnlyList<string> value)
            return value;

        return new List<string>(this.collection);
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return string.Join(Environment.NewLine, this.collection);
    }
}