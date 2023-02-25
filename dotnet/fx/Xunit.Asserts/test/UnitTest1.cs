using Xunit;

namespace Bearz.Xunit.Asserts.Tests;

public class UnitTest1
{
    private static readonly IAssert s_assert = FlexAssert.Default;

    [Fact]
    public void Test1()
    {
        const int i = 5, j = 6;
        s_assert.NotEqual(i, j);
    }
}