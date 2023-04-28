using System;

using Xunit.Sdk;

namespace Xunit;

[TestFrameworkDiscoverer(
    "Xunit.Sdk.BearzTestFrameworkTypeDiscoverer",
    "Bearz.Xunit.Core")]
[AttributeUsage(
    System.AttributeTargets.Assembly,
    Inherited = false,
    AllowMultiple = false)]
public sealed class BearzTestFrameworkAttribute : System.Attribute,
    ITestFrameworkAttribute
{
    public BearzTestFrameworkAttribute()
    {
    }

    public Type? CustomFrameworkType { get; set; }
}