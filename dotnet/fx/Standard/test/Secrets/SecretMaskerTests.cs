using Bearz.Secrets;

namespace Tests;

public static class SecretMaskerTests
{
    private const string Filter = "**********";

    [UnitTest]
    public static void Mask()
    {
        var masker = SecretMasker.Default;
        masker.Add("password");
        Assert.Equal(Filter, masker.Mask("password"));
    }

    [UnitTest]
    public static void MultilineMask()
    {
        var masker = SecretMasker.Default;
        masker.Add("password");
        var content = """
This is a very complex
makes @#$13ds with password
testsdfwes
""";
        var actual = content.Replace("password", Filter);
        Assert.Equal(actual, masker.Mask(content));
    }

    [UnitTest]
    public static void MultipleSecretMask()
    {
        var masker = SecretMasker.Default;
        masker.Add("password");
        masker.Add("Hello");
        Assert.Equal($"{Filter} my name is {Filter} .", masker.Mask("Hello my name is password ."));
    }
}