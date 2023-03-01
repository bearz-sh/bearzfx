using System.Text;

namespace Bearz.Extra.Memory;

public ref struct SpanBuilder<T>
{
    private readonly int initialCapacity = 10;

    private Span<T> span;

    public SpanBuilder()
    {
        this.span = new Span<T>(new T[this.initialCapacity]);
    }

    public SpanBuilder(int initialCapacity)
    {
        this.initialCapacity = initialCapacity;
        this.span = new Span<T>(new T[initialCapacity]);
    }

    public int Length { get; internal set; }

    public bool IsEmpty => this.Length == 0;

    private int Position { get; set; }

    public T this[int index]
    {
        get => this.span[index];
        set => this.span[index] = value;
    }

    public void Append(T item)
    {
        this.Grow();
        this.span[this.Position] = item;
        this.Position++;
        this.Length++;
    }

    public void Append(Span<T> items)
    {
        this.Grow(items.Length);
        items.CopyTo(this.span.Slice(this.Position));
        this.Increment(items.Length);
    }

    public void Append(params T[] items)
        => this.Append(items.AsSpan());

    public void Append(ReadOnlySpan<T> items)
    {
        this.Grow(items.Length);
        items.CopyTo(this.span.Slice(this.Position));
        this.Increment(items.Length);
    }

    public void AppendJoin(T separator, Span<T> item1)
    {
        if (this.Position > 0)
        {
            this.Grow();
            this.span[this.Position] = separator;
            this.Increment();
        }

        this.Grow(item1.Length);
        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
    }

    public void AppendJoin(T separator, ReadOnlySpan<T> item1)
    {
        if (this.Position > 0)
        {
            this.Grow();
            this.span[this.Position] = separator;
            this.Increment();
        }

        this.Grow(item1.Length);
        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
    }

    public void AppendJoin(T separator, Span<T> item1, Span<T> item2)
    {
        int l = item1.Length + item2.Length;
        if (this.Position > 0)
        {
            this.Grow(l + 1);
            this.span[this.Position] = separator;
            this.Increment();
        }
        else
        {
            this.Grow(l);
        }

        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
    }

    public void AppendJoin(T separator, ReadOnlySpan<T> item1, ReadOnlySpan<T> item2)
    {
        int l = item1.Length + item2.Length;
        if (this.Position > 0)
        {
            this.Grow(l + 1);
            this.span[this.Position] = separator;
            this.Increment();
        }
        else
        {
            this.Grow(l);
        }

        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
    }

    public void AppendJoin(T separator, Span<T> item1, Span<T> item2, Span<T> item3)
    {
        int l = item1.Length + item2.Length + item3.Length;
        if (this.Position > 0)
        {
            this.Grow(l + 2);
            this.span[this.Position] = separator;
            this.Increment();
        }
        else
        {
            this.Grow(l + 1);
        }

        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item3.CopyTo(this.span.Slice(this.Position));
        this.Increment(item3.Length);
    }

    public void AppendJoin(T separator, ReadOnlySpan<T> item1, ReadOnlySpan<T> item2, ReadOnlySpan<T> item3)
    {
        int l = item1.Length + item2.Length + item3.Length;
        if (this.Position > 0)
        {
            this.Grow(l + 2);
            this.span[this.Position] = separator;
            this.Increment();
        }
        else
        {
            this.Grow(l + 1);
        }

        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item3.CopyTo(this.span.Slice(this.Position));
        this.Increment(item3.Length);
    }

    public void AppendJoin(T separator, Span<T> item1, Span<T> item2, Span<T> item3, Span<T> item4)
    {
        int l = item1.Length + item2.Length + item3.Length;
        if (this.Position > 0)
        {
            this.Grow(l + 2);
            this.span[this.Position] = separator;
            this.Increment();
        }
        else
        {
            this.Grow(l + 1);
        }

        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item3.CopyTo(this.span.Slice(this.Position));
        this.Increment(item3.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item4.CopyTo(this.span.Slice(this.Position));
        this.Increment(item4.Length);
    }

    public void AppendJoin(T separator, ReadOnlySpan<T> item1, ReadOnlySpan<T> item2, ReadOnlySpan<T> item3, ReadOnlySpan<T> item4)
    {
        int l = item1.Length + item2.Length + item3.Length;
        if (this.Position > 0)
        {
            this.Grow(l + 2);
            this.span[this.Position] = separator;
            this.Increment();
        }
        else
        {
            this.Grow(l + 1);
        }

        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item3.CopyTo(this.span.Slice(this.Position));
        this.Increment(item3.Length);
        this.span[this.Position] = separator;
        this.Increment();
        item4.CopyTo(this.span.Slice(this.Position));
        this.Increment(item4.Length);
    }

    public void AppendJoin(ReadOnlySpan<T> separator, Span<T> item1)
    {
        if (this.Position > 0)
        {
            this.Grow(separator.Length);
            separator.CopyTo(this.span.Slice(this.Position));
            this.Increment(separator.Length);
        }

        this.Grow(item1.Length);
        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
    }

    public void AppendJoin(ReadOnlySpan<T> separator, Span<T> item1, Span<T> item2)
    {
        if (this.Position > 0)
        {
            this.Grow(separator.Length);
            separator.CopyTo(this.span.Slice(this.Position));
            this.Increment(separator.Length);
        }

        this.Grow(item1.Length + item2.Length + separator.Length);
        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        separator.CopyTo(this.span.Slice(this.Position));
        this.Increment(separator.Length);
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
    }

    public void AppendJoin(ReadOnlySpan<T> separator, Span<T> item1, Span<T> item2, Span<T> item3)
    {
        if (this.Position > 0)
        {
            this.Grow(separator.Length);
            separator.CopyTo(this.span.Slice(this.Position));
            this.Increment(separator.Length);
        }

        this.Grow(item1.Length + item2.Length + item3.Length + (separator.Length * 2));
        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        separator.CopyTo(this.span.Slice(this.Position));
        this.Increment(separator.Length);
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
        separator.CopyTo(this.span.Slice(this.Position));
        this.Increment(separator.Length);
        item3.CopyTo(this.span.Slice(this.Position));
        this.Increment(item3.Length);
    }

    public void AppendJoin(ReadOnlySpan<T> separator, Span<T> item1, Span<T> item2, Span<T> item3, Span<T> item4)
    {
        if (this.Position > 0)
        {
            this.Grow(separator.Length);
            separator.CopyTo(this.span.Slice(this.Position));
            this.Increment(separator.Length);
        }

        this.Grow(item1.Length + item2.Length + item3.Length + item4.Length + (separator.Length * 3));
        item1.CopyTo(this.span.Slice(this.Position));
        this.Increment(item1.Length);
        separator.CopyTo(this.span.Slice(this.Position));
        this.Increment(separator.Length);
        item2.CopyTo(this.span.Slice(this.Position));
        this.Increment(item2.Length);
        separator.CopyTo(this.span.Slice(this.Position));
        this.Increment(separator.Length);
        item3.CopyTo(this.span.Slice(this.Position));
        this.Increment(item3.Length);
        separator.CopyTo(this.span.Slice(this.Position));
        this.Increment(separator.Length);
        item4.CopyTo(this.span.Slice(this.Position));
        this.Increment(item4.Length);
    }

    public void Clear()
    {
        this.span = new Span<T>(new T[this.initialCapacity]);
    }

    public void Insert(int index, T item)
    {
        this.Grow();
        this.span.Slice(index, this.Length - index).CopyTo(this.span.Slice(index + 1));
        this.span[index] = item;
        this.Increment();
    }

    public void Insert(int index, Span<T> items)
    {
        this.Grow(items.Length);
        this.span.Slice(index, this.Length - index).CopyTo(this.span.Slice(index + items.Length));
        items.CopyTo(this.span.Slice(index));
        this.Increment(items.Length);
    }

    public void Insert(int index, params T[] items)
        => this.Insert(index, items.AsSpan());

    public void Insert(int index, ReadOnlySpan<T> items)
    {
        this.Grow(items.Length);
        this.span.Slice(index, this.Length - index).CopyTo(this.span.Slice(index + items.Length));
        items.CopyTo(this.span.Slice(index));
        this.Increment(items.Length);
    }

    public void Prepend(T item)
    {
        this.Grow();
        this.span.Slice(1).CopyTo(this.span);
        this.span[0] = item;
        this.Increment();
    }

    public void Prepend(Span<T> items)
    {
        this.Grow(items.Length);
        this.span.Slice(items.Length).CopyTo(this.span);
        items.CopyTo(this.span);
        this.Increment(items.Length);
    }

    public void Prepend(params T[] items)
        => this.Prepend(items.AsSpan());

    public void Prepend(ReadOnlySpan<T> items)
    {
        this.Grow(items.Length);
        this.span.Slice(items.Length).CopyTo(this.span);
        items.CopyTo(this.span);
        this.Increment(items.Length);
    }

    public void CopyTo(Span<T> destination)
        => this.span.Slice(0, this.Length).CopyTo(destination);

    public Span<T> ToSpan()
        => this.span.Slice(0, this.Length);

    private void Decrement()
    {
        this.Position--;
        this.Length--;
    }

    private void Decrement(int count)
    {
        this.Position -= count;
        this.Length -= count;
    }

    private void Increment()
    {
        this.Position++;
        this.Length++;
    }

    private void Increment(int count)
    {
        this.Position += count;
        this.Length += count;
    }

    private void Grow(int count)
    {
        if (this.Position + count < this.span.Length)
            return;

        var copy = new T[Math.Max(this.span.Length * 2, this.span.Length + count)];
        this.span.CopyTo(copy);
        this.span = copy;
    }

    private void Grow()
    {
        if (this.Position != this.span.Length)
            return;

        var copy = new T[this.span.Length * 2];
        this.span.CopyTo(copy);
        this.span = copy;
    }
}