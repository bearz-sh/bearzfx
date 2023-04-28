using System;
using System.Diagnostics.CodeAnalysis;

namespace Bearz.Secrets;

public interface IMask
{
    [return: NotNullIfNotNull("value")]
    string? Mask(string? value);

    ReadOnlySpan<char> Mask(ReadOnlySpan<char> value);
}