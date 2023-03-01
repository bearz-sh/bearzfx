using System.Collections.ObjectModel;

using static Bearz.Extra.Arrays.ArrayOps;

namespace Tests;

public static class ArrayOps_Tests
{
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
}