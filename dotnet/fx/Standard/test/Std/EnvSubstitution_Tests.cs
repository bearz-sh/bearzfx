using Bearz.Std;

namespace Test.Std;

// ReSharper disable once InconsistentNaming
public class EnvSubstitution_Tests
{
    [UnitTest]
    public void EvaluateNothing(IAssert assert)
    {
        var result = EnvSubstitution.Evaluate("Hello World");
        assert.Equal("Hello World", result);
    }

    [UnitTest]
    public void EvaluateEscapedName(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvSubstitution.Evaluate("Hello \\$WORD");
        assert.Equal("Hello $WORD", result);

        result = EnvSubstitution.Evaluate("Hello $WORD\\_SUN");
        assert.Equal("Hello World_SUN", result);
    }

    [UnitTest]
    public void EvaluateDoubleBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");
        Environment.SetEnvironmentVariable("HELLO", "Hello");

        var result = EnvSubstitution.Evaluate("$HELLO $WORD");
        assert.Equal("Hello World", result);

        result = EnvSubstitution.Evaluate("$HELLO$WORD!");
        assert.Equal("HelloWorld!", result);
    }

    [UnitTest]
    public void EvaluateSingleWindowsVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvSubstitution.Evaluate("Hello %WORD%");
        assert.Equal("Hello World", result);

        result = EnvSubstitution.Evaluate("Hello test%WORD%:");
        assert.Equal("Hello testWorld:", result);

        result = EnvSubstitution.Evaluate("%WORD%");
        assert.Equal("World", result);

        result = EnvSubstitution.Evaluate("%WORD%  ");
        assert.Equal("World  ", result);

        result = EnvSubstitution.Evaluate(" \n%WORD%  ");
        assert.Equal(" \nWorld  ", result);
    }

    [UnitTest]
    public void EvaluateSingleBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvSubstitution.Evaluate("Hello $WORD");
        assert.Equal("Hello World", result);

        result = EnvSubstitution.Evaluate("Hello test$WORD:");
        assert.Equal("Hello testWorld:", result);

        result = EnvSubstitution.Evaluate("$WORD");
        assert.Equal("World", result);

        result = EnvSubstitution.Evaluate("$WORD  ");
        assert.Equal("World  ", result);

        result = EnvSubstitution.Evaluate(" \n$WORD  ");
        assert.Equal(" \nWorld  ", result);
    }

    [UnitTest]
    public void EvaluateSingleInterpolatedBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var result = EnvSubstitution.Evaluate("Hello ${WORD}");
        assert.Equal("Hello World", result);

        result = EnvSubstitution.Evaluate("Hello test${WORD}:");
        assert.Equal("Hello testWorld:", result);

        result = EnvSubstitution.Evaluate("${WORD}");
        assert.Equal("World", result);

        result = EnvSubstitution.Evaluate("${WORD}  ");
        assert.Equal("World  ", result);

        result = EnvSubstitution.Evaluate(" \n$WORD  ");
        assert.Equal(" \nWorld  ", result);
    }

    [UnitTest]
    public void UseDefaultValueForBashVar(IAssert assert)
    {
        // assert state
        assert.False(Env.Has("WORD2"));

        var result = EnvSubstitution.Evaluate("${WORD2:-World}");
        assert.Equal("World", result);
        assert.False(Env.Has("WORD2"));
    }

    [UnitTest]
    public void SetEnvValueWithBashVarWhenNull(IAssert assert)
    {
        // assert state
        assert.False(Env.Has("WORD3"));

        var result = EnvSubstitution.Evaluate("${WORD3:=World}");
        assert.Equal("World", result);
        assert.True(Env.Has("WORD3"));
        assert.Equal("World", Env.Get("WORD3"));
    }

    [UnitTest]
    public void ThrowOnMissingBashVar(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        var ex = assert.Throws<EnvSubstitutionException>(() =>
        {
            EnvSubstitution.Evaluate("Hello ${WORLD:?WORLD must be set}");
        });

        assert.Equal("WORLD must be set", ex.Message);

        ex = assert.Throws<EnvSubstitutionException>(() =>
        {
            EnvSubstitution.Evaluate("Hello ${WORLD}");
        });

        assert.Equal("Bad substitution, variable WORLD is not set.", ex.Message);

        ex = assert.Throws<EnvSubstitutionException>(() =>
        {
            EnvSubstitution.Evaluate("Hello $WORLD");
        });

        assert.Equal("Bad substitution, variable WORLD is not set.", ex.Message);
    }

    [UnitTest]
    public void UnclosedToken_Exception(IAssert assert)
    {
        Environment.SetEnvironmentVariable("WORD", "World");

        assert.Throws<EnvSubstitutionException>(() =>
        {
            EnvSubstitution.Evaluate("Hello ${WORD");
        });

        assert.Throws<EnvSubstitutionException>(() =>
        {
            EnvSubstitution.Evaluate("Hello %WORD");
        });
    }
}