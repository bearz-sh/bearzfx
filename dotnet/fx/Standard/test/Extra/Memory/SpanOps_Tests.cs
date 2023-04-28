using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Bearz.Extra.Memory;

namespace Tests;

public static class SpanOps_Tests
{
    [UnitTest]
    public static void Verify_ConvertToString_Characters(IAssert assert)
    {
        var span = "Hello World".AsSpan();
        var str = SpanOps.ConvertToString(span);
        assert.Equal("Hello World", str);
    }

    [UnitTest]
    public static void Verify_ConvertToString_Bytes(IAssert assert)
    {
        var span = "Hello World"u8;
        var str = SpanOps.ConvertToString(span);
        assert.Equal("Hello World", str);
    }

    [UnitTest]
    public static void Verify_Pop(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });

        assert.Equal(3, SpanOps.Pop(ref span));
        assert.Equal(2, SpanOps.Pop(ref span));
        assert.Equal(1, SpanOps.Pop(ref span));

        Assert.Throws<ArgumentException>(() =>
        {
            var span = new Span<int>(Array.Empty<int>());
            SpanOps.Pop(ref span);
        });

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });

        assert.Equal(3, SpanOps.Pop(ref ro));
        assert.Equal(2, SpanOps.Pop(ref ro));
        assert.Equal(1, SpanOps.Pop(ref ro));

        Assert.Throws<ArgumentException>(() =>
        {
            var ro = new ReadOnlySpan<int>(Array.Empty<int>());
            SpanOps.Pop(ref ro);
        });
    }

    [UnitTest]
    public static void Verify_Shift(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });

        assert.Equal(1, SpanOps.Shift(ref span));
        assert.Equal(2, SpanOps.Shift(ref span));
        assert.Equal(3, SpanOps.Shift(ref span));

        Assert.Throws<ArgumentException>(() =>
        {
            var span = new Span<int>(Array.Empty<int>());
            SpanOps.Pop(ref span);
        });

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });

        assert.Equal(1, SpanOps.Shift(ref ro));
        assert.Equal(2, SpanOps.Shift(ref ro));
        assert.Equal(3, SpanOps.Shift(ref ro));

        Assert.Throws<ArgumentException>(() =>
        {
            var ro = new ReadOnlySpan<int>(Array.Empty<int>());
            SpanOps.Pop(ref ro);
        });
    }

    [UnitTest]
    public static void Append_SingleValue(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref span, 4);

        assert.Equal(4, span.Length);
        assert.Equal(4, span[3]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref ro, 4);

        assert.Equal(4, ro.Length);
        assert.Equal(4, ro[3]);
    }

    [UnitTest]
    public static void Append_Span(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });
        var span2 = new Span<int>(new[] { 4, 5, 6 });
        SpanOps.Append(ref span, span2);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref ro, span2);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[3]);
        assert.Equal(5, ro[4]);
        assert.Equal(6, ro[5]);
    }

    [UnitTest]
    public static void Append_Params(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref span, 4, 5, 6);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref ro, 4, 5, 6);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[3]);
        assert.Equal(5, ro[4]);
        assert.Equal(6, ro[5]);
    }

    [UnitTest]
    public static void Append_IEnumerable(IAssert assert)
    {
        IEnumerable<int> array = new[] { 4, 5, 6 };
        IEnumerable<int> list = new List<int>() { 4, 5, 6 };
        IEnumerable<int> enumerable = Enumerable.Range(4, 3);
        IEnumerable<int> collection = new Collection<int>() { 4, 5, 6 };

        var span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref span, array);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref span, list);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref span, collection);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        span = new Span<int>(new[] { 1, 2, 3 });

        // ReSharper disable once PossibleMultipleEnumeration
        SpanOps.Append(ref span, enumerable);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref ro, array);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[3]);
        assert.Equal(5, ro[4]);
        assert.Equal(6, ro[5]);

        ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref ro, list);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[3]);
        assert.Equal(5, ro[4]);
        assert.Equal(6, ro[5]);

        ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Append(ref ro, collection);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[3]);
        assert.Equal(5, ro[4]);
        assert.Equal(6, ro[5]);

        ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });

        // ReSharper disable once PossibleMultipleEnumeration
        SpanOps.Append(ref ro, enumerable);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[3]);
        assert.Equal(5, ro[4]);
        assert.Equal(6, ro[5]);
    }

    [UnitTest]
    public static void Prepend_SingleValue(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref span, 4);

        assert.Equal(4, span.Length);
        assert.Equal(4, span[0]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref ro, 4);

        assert.Equal(4, ro.Length);
        assert.Equal(4, ro[0]);
    }

    [UnitTest]
    public static void Prepend_Span(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });
        var span2 = new Span<int>(new[] { 4, 5, 6 });
        SpanOps.Prepend(ref span, span2);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref ro, span2);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[0]);
        assert.Equal(5, ro[1]);
        assert.Equal(6, ro[2]);
    }

    [UnitTest]
    public static void Prepend_Params(IAssert assert)
    {
        var span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref span, 4, 5, 6);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref ro, 4, 5, 6);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[0]);
        assert.Equal(5, ro[1]);
        assert.Equal(6, ro[2]);
    }

    [UnitTest]
    public static void Prepend_IEnumerable(IAssert assert)
    {
        IEnumerable<int> array = new[] { 4, 5, 6 };
        IEnumerable<int> list = new List<int>() { 4, 5, 6 };
        IEnumerable<int> enumerable = Enumerable.Range(4, 3);
        IEnumerable<int> collection = new Collection<int>() { 4, 5, 6 };

        var span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref span, array);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref span, list);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        span = new Span<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref span, collection);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        span = new Span<int>(new[] { 1, 2, 3 });

        // ReSharper disable once PossibleMultipleEnumeration
        SpanOps.Prepend(ref span, enumerable);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        var ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref ro, array);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[0]);
        assert.Equal(5, ro[1]);
        assert.Equal(6, ro[2]);

        ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref ro, list);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[0]);
        assert.Equal(5, ro[1]);
        assert.Equal(6, ro[2]);

        ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });
        SpanOps.Prepend(ref ro, collection);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[0]);
        assert.Equal(5, ro[1]);
        assert.Equal(6, ro[2]);

        ro = new ReadOnlySpan<int>(new[] { 1, 2, 3 });

        // ReSharper disable once PossibleMultipleEnumeration
        SpanOps.Prepend(ref ro, enumerable);

        assert.Equal(6, ro.Length);
        assert.Equal(4, ro[0]);
        assert.Equal(5, ro[1]);
        assert.Equal(6, ro[2]);
    }
}