using System.Runtime.InteropServices;

namespace Test;

public partial class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Assert.NotEqual(10, 12);
    }
}