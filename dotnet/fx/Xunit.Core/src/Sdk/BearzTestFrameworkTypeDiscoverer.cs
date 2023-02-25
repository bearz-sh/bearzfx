using System;

using Xunit.Abstractions;

namespace Xunit.Sdk;

public class BearzTestFrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
{
    public Type GetTestFrameworkType(IAttributeInfo attribute)
    {
        var frameworkType = attribute.GetNamedArgument<Type?>("CustomFrameworkType");
        return frameworkType ?? typeof(BearzTestFramework);
    }
}