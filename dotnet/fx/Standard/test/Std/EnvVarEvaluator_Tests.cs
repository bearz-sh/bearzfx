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
    public void EvaluateEscapedName(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvVarEvaluator.Evaluate("Hello \\$WORD");
        assert.Equal("Hello $WORD", result);

        result = EnvVarEvaluator.Evaluate("Hello $WORD\\_SUN");
        assert.Equal("Hello World_SUN", result);
    }

    [UnitTest]
    public void EvaluateDoubleBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");
        Environment.SetEnvironmentVariable("HELLO", "Hello");

        var result = EnvVarEvaluator.Evaluate("$HELLO $WORD");
        assert.Equal("Hello World", result);

        result = EnvVarEvaluator.Evaluate("$HELLO$WORD!");
        assert.Equal("HelloWorld!", result);
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
    public void EvaluateSingleInterpolatedBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvVarEvaluator.Evaluate("Hello ${WORD}");
        assert.Equal("Hello World", result);

        result = EnvVarEvaluator.Evaluate("Hello test${WORD}:");
        assert.Equal("Hello testWorld:", result);

        result = EnvVarEvaluator.Evaluate("${WORD}");
        assert.Equal("World", result);

        result = EnvVarEvaluator.Evaluate("${WORD}  ");
        assert.Equal("World  ", result);

        result = EnvVarEvaluator.Evaluate(" \n$WORD  ");
        assert.Equal(" \nWorld  ", result);
    }

    [UnitTest]
    public void UseDefaultValueForBashVar(IAssert assert)
    {
        // assert state
        assert.False(Env.Has("WORD2"));

        var result = EnvVarEvaluator.Evaluate("${WORD2:-World}");
        assert.Equal("World", result);
        assert.False(Env.Has("WORD2"));
    }

    [UnitTest]
    public void SetEnvValueWithBashVarWhenNull(IAssert assert)
    {
        // assert state
        assert.False(Env.Has("WORD3"));

        var result = EnvVarEvaluator.Evaluate("${WORD3:=World}");
        assert.Equal("World", result);
        assert.True(Env.Has("WORD3"));
        assert.Equal("World", Env.Get("WORD3"));
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