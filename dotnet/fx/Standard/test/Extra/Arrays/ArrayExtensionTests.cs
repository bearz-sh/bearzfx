using Bearz.Extra.Arrays;

using static Bearz.Extra.Arrays.ArrayExtensions;

namespace Tests;

public static class ArrayExtensionTests
{
    [UnitTest]
    public static void Clear(IAssert assert)
    {
        var array = new[] { 1, 2, 3 };
        assert.Equal(3, array.Length);
        assert.Equal(1, array[0]);
        assert.Equal(2, array[1]);
        assert.Equal(3, array[2]);

        array.Clear();
        assert.Equal(3, array.Length);
        assert.Equal(0, array[0]);
        assert.Equal(0, array[1]);
        assert.Equal(0, array[2]);
    }
}