using System;
using System.Collections.Generic;

namespace Bearz.Secrets;

public interface ISecretGenerator
{
    ISecretGenerator Add(char character);

    ISecretGenerator Add(IEnumerable<char> characters);

    ISecretGenerator SetValidator(Func<char[], bool> validator);

    char[] Generate(int length);

    char[] Generate(int length, IList<char>? characters, Func<char[], bool>? validator);
}