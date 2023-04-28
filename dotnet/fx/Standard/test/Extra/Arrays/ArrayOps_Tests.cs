using System.Collections.ObjectModel;

using static Bearz.Extra.Arrays.ArrayOps;

namespace Tests;

public static class ArrayOps_Tests
{
    [UnitTest]
    public static void Verify_Concat()
    {
        var array1 = new[] { 1 };
        var array2 = new[] { 2 };
        var array3 = new[] { 3 };
        var array4 = new[] { 4 };

        var array = Concat(array1, array2, array3, array4);
        Assert.Equal(4, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
        Assert.Equal(4, array[3]);

        array = Concat(array1, array2);
        Assert.Equal(2, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);

        array = Concat(array1, array2, array3);
        Assert.Equal(3, array.Length);
        Assert.Equal(1, array[0]);
        Assert.Equal(2, array[1]);
        Assert.Equal(3, array[2]);
    }

    [UnitTest]
    public static void Verify_Slice(IAssert assert)
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        var slice = Slice(array, 1, 3);

        assert.Equal(3, slice.Length);
        assert.Equal(2, slice[0]);
        assert.Equal(3, slice[1]);

        slice = Slice(array, 1);
        assert.Equal(4, slice.Length);
        assert.Equal(2, slice[0]);
        assert.Equal(5, slice[3]);
    }

    [UnitTest]
    public static void Verify_Segment(IAssert assert)
    {
        var array = new[] { 1, 2, 3, 4, 5 };
        var segment = AsSegment(array, 1, 3);

        assert.Equal(1, segment.Offset);
        assert.Equal(3, segment.Count);
        assert.Equal(2, segment.First());
        assert.Equal(4, segment.Last());

        segment = AsSegment(array, 1);
        assert.Equal(1, segment.Offset);
        assert.Equal(4, segment.Count);
        assert.Equal(2, segment.First());
        assert.Equal(5, segment.Last());
    }

    [UnitTest]
    public static void Verify_Pop(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        var item = Pop(ref array);

        assert.Equal(3, item);
        assert.Equal(2, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(2, array[1]);
    }

    [UnitTest]
    public static void Verify_Shift(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        var item = Shift(ref array);

        assert.Equal(1, item);
        assert.Equal(2, array.Length);
        assert.Equal(2, array[0]);
        assert.Equal(3, array[1]);
    }

    [UnitTest]
    public static void Verify_Swap(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        Swap(array, 0, 2);

        assert.Equal(3, array.Length);
        assert.Equal(3, array[0]);
        assert.Equal(2, array[1]);
        assert.Equal(1, array[2]);
    }

    [UnitTest]
    public static void Verify_Equal(IAssert assert)
    {
        var array1 = new[] { 1, 2, 3 };
        var array2 = new[] { 1, 2, 3 };
        var array3 = new[] { 1, 2, 3, 4 };
        var array4 = new[] { 4, 2, 4 };

        assert.True(Equal(array1, array1));
        assert.True(Equal(array1, array2));
        assert.False(Equal(array1, array3));
        assert.False(Equal(array1, array4));

        assert.True(Equal(array1, array1, (a, b) => a.CompareTo(b)));
        assert.True(Equal(array1, array2, (a, b) => a.CompareTo(b)));
        assert.False(Equal(array1, array3, (a, b) => a.CompareTo(b)));

        // treat 4 as a wildcard where it matches any value
        var eq = new IntEqualityComparer();
        assert.True(Equal(array1, array1, eq));
        assert.True(Equal(array1, array2, eq));
        assert.False(Equal(array1, array3, eq));
        assert.True(Equal(array1, array4, eq));

        // treat 4 as a wildcard where it matches any value
        var cp = new IntComparer();
        assert.True(Equal(array1, array1, cp));
        assert.True(Equal(array1, array2, cp));
        assert.False(Equal(array1, array3, cp));
        assert.True(Equal(array1, array4, cp));
    }

    [UnitTest]
    public static void Verify_Grow(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        Grow(ref array, 2);

        assert.Equal(5, array.Length);
    }

    [UnitTest]
    public static void Verify_Shrink(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        Shrink(ref array, 2);

        assert.Equal(1, array.Length);
    }

    [UnitTest]
    public static void Verify_ShrinkBy(IAssert assert)
    {
        var array = new int[32];
        ShrinkBy(ref array, 16);

        assert.Equal(16, array.Length);

        array = new int[32];
        ShrinkBy(ref array, 0);
        assert.Equal(0, array.Length);

        array = new int[32];
        ShrinkBy(ref array, 32);
        assert.Equal(32, array.Length);
    }

    [UnitTest]
    public static void Verify_Rent_And_Return(IAssert assert)
    {
        var array = Rent<int>(10);
        assert.True(array.Length >= 10);
        assert.Equal(0, array[0]);

        array[0] = 1;
        Return(array);
        assert.Equal(1, array[0]);

        array = Rent<int>(10);
        assert.True(array.Length >= 10);
        array[0] = 1;

        Return(array, true);
        assert.Equal(0, array[0]);
    }

    [UnitTest]
    public static void Verify_Append_Single(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        Append(ref array, 4);

        assert.Equal(4, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(2, array[1]);
        assert.Equal(3, array[2]);
        assert.Equal(4, array[3]);
    }

    [UnitTest]
    public static void Verify_Append_Arrays(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        var items = new[] { 4, 5, 6 };
        Append(ref array, items);

        assert.Equal(6, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(2, array[1]);
        assert.Equal(3, array[2]);
        assert.Equal(4, array[3]);
        assert.Equal(5, array[4]);
        assert.Equal(6, array[5]);

        array = new[] { 1, 2, 3 };
        Append(ref array, items.AsSpan(1));

        assert.Equal(5, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(2, array[1]);
        assert.Equal(3, array[2]);
        assert.Equal(5, array[3]);
        assert.Equal(6, array[4]);
    }

    [UnitTest]
    public static void Verify_Append_Enumerable(IAssert assert)
    {
        IEnumerable<int> array = new[] { 4, 5, 6 };
        IEnumerable<int> list = new List<int>() { 4, 5, 6 };
        IEnumerable<int> enumerable = Enumerable.Range(4, 3);
        IEnumerable<int> collection = new Collection<int>() { 4, 5, 6 };

        var span = new[] { 1, 2, 3 };
        Append(ref span, array);

        assert.Equal(6, span.Length);
        assert.Equal(1, span[0]);
        assert.Equal(2, span[1]);
        assert.Equal(3, span[2]);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        span = new[] { 1, 2, 3 };
        Append(ref span, list);

        assert.Equal(6, span.Length);
        assert.Equal(1, span[0]);
        assert.Equal(2, span[1]);
        assert.Equal(3, span[2]);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        span = new[] { 1, 2, 3 };
        Append(ref span, collection);

        assert.Equal(6, span.Length);
        assert.Equal(1, span[0]);
        assert.Equal(2, span[1]);
        assert.Equal(3, span[2]);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);

        span = new[] { 1, 2, 3 };
        Append(ref span, enumerable);

        assert.Equal(6, span.Length);
        assert.Equal(1, span[0]);
        assert.Equal(2, span[1]);
        assert.Equal(3, span[2]);
        assert.Equal(4, span[3]);
        assert.Equal(5, span[4]);
        assert.Equal(6, span[5]);
    }

    [UnitTest]
    public static void Verify_Insert_Single(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        Insert(ref array, 1, 4);

        assert.Equal(4, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(4, array[1]);
        assert.Equal(2, array[2]);
        assert.Equal(3, array[3]);
    }

    [UnitTest]
    public static void Verify_Insert_Arrays(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        var items = new[] { 4, 5, 6 };
        Insert(ref array, 1, items);

        assert.Equal(6, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(4, array[1]);
        assert.Equal(5, array[2]);
        assert.Equal(6, array[3]);
        assert.Equal(2, array[4]);
        assert.Equal(3, array[5]);
    }

    [UnitTest]
    public static void Verify_Prepend_Single(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        Prepend(ref array, 4);

        assert.Equal(4, array.Length);
        assert.Equal(4, array[0]);
        assert.Equal(1, array[1]);
        assert.Equal(2, array[2]);
        assert.Equal(3, array[3]);
    }

    [UnitTest]
    public static void Verify_Prepend_Arrays(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        var items = new[] { 4, 5, 6 };
        Prepend(ref array, items);

        assert.Equal(6, array.Length);
        assert.Equal(4, array[0]);
        assert.Equal(5, array[1]);
        assert.Equal(6, array[2]);

        array = new[] { 1, 2, 3 };
        Prepend(ref array, items.AsSpan(1));

        assert.Equal(5, array.Length);
        assert.Equal(5, array[0]);
        assert.Equal(6, array[1]);
    }

    [UnitTest]
    public static void Verify_Prepend_Enumerable(IAssert assert)
    {
        IEnumerable<int> array = new[] { 4, 5, 6 };
        IEnumerable<int> list = new List<int>() { 4, 5, 6 };
        IEnumerable<int> enumerable = Enumerable.Range(4, 3);
        IEnumerable<int> collection = new Collection<int>() { 4, 5, 6 };

        var span = new[] { 1, 2, 3 };
        Prepend(ref span, array);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        span = new[] { 1, 2, 3 };
        Prepend(ref span, list);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        span = new[] { 1, 2, 3 };
        Prepend(ref span, collection);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);

        span = new[] { 1, 2, 3 };

        // ReSharper disable once PossibleMultipleEnumeration
        Prepend(ref span, enumerable);

        assert.Equal(6, span.Length);
        assert.Equal(4, span[0]);
        assert.Equal(5, span[1]);
        assert.Equal(6, span[2]);
    }

    private class IntComparer : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            return x == y || x == 4 || y == 4 ? 0 : 1;
        }
    }

    private class IntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y || x == 4 || y == 4;
        }

        public int GetHashCode(int obj)
        {
            throw new NotImplementedException();
        }
    }
}