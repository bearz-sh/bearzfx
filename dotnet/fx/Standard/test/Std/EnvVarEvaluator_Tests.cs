using Bearz.Std;

namespace Test.Std;

public class EnvVarEvaluator_Tests
{
    [UnitTest]
    public void EvaluateNothing(IAssert assert)
    {
        var result = EnvVarEvaluator.Evaluate("Hello World");
        assert.Equal("Hello World", result);
    }

    [UnitTest]
    public void EvaluateSingleWindowsVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvVarEvaluator.Evaluate("Hello %WORD%", true);
        assert.Equal("Hello World", result);

        result = EnvVarEvaluator.Evaluate("Hello test%WORD%:", true);
        assert.Equal("Hello testWorld:", result);

        result = EnvVarEvaluator.Evaluate("%WORD%", true);
        assert.Equal("World", result);

        result = EnvVarEvaluator.Evaluate("%WORD%  ", true);
        assert.Equal("World  ", result);

        result = EnvVarEvaluator.Evaluate(" \n%WORD%  ", true);
        assert.Equal(" \nWorld  ", result);
    }

    [UnitTest]
    public void EvaluateSingleBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvVarEvaluator.Evaluate("Hello $WORD");
        assert.Equal("Hello World", result);

        result = EnvVarEvaluator.Evaluate("Hello test$WORD:");
        assert.Equal("Hello testWorld:", result);

        result = EnvVarEvaluator.Evaluate("$WORD");
        assert.Equal("World", result);

        result = EnvVarEvaluator.Evaluate("$WORD  ");
        assert.Equal("World  ", result);

        result = EnvVarEvaluator.Evaluate(" \n$WORD  ");
        assert.Equal(" \nWorld  ", result);
    }

    [UnitTest]
    public void ThrowOnMissingBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var ex = assert.Throws<EnvVarSubstitutionException>(() =>
        {
            EnvVarEvaluator.Evaluate("Hello ${WORLD:?WORLD must be set}");
        });

        assert.Equal("WORLD must be set", ex.Message);

        ex = assert.Throws<EnvVarSubstitutionException>(() =>
        {
            EnvVarEvaluator.Evaluate("Hello ${WORLD}");
        });

        assert.Equal("Bad substitution, variable WORLD is not set.", ex.Message);

        ex = assert.Throws<EnvVarSubstitutionException>(() =>
        {
            EnvVarEvaluator.Evaluate("Hello $WORLD");
        });

        assert.Equal("Bad substitution, variable WORLD is not set.", ex.Message);
    }

    [UnitTest]
    public void UnclosedToken_Exception(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        assert.Throws<EnvVarParseTokenException>(() =>
        {
            EnvVarEvaluator.Evaluate("Hello ${WORD");
        });

        assert.Throws<EnvVarParseTokenException>(() =>
        {
            EnvVarEvaluator.Evaluate("Hello %WORD", true);
        });
    }
}