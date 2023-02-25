using System;
using System.Collections.Generic;
using System.Text;

using Xunit.Abstractions;

namespace Xunit.Sdk;

public class BearzTestDiscoverer : FactDiscoverer
{
    public BearzTestDiscoverer(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    public static ITestServiceProviderLocator? ServiceProviderLocator { get; set; }

    public override IEnumerable<IXunitTestCase> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        if (factAttribute is not ReflectionAttributeInfo { Attribute: TestAttribute })
            return base.Discover(discoveryOptions, testMethod, factAttribute);

        IXunitTestCase testCase;

        if (testMethod.Method.IsGenericMethodDefinition)
        {
            testCase = this.ErrorTestCase(
                discoveryOptions,
                testMethod,
                "[Fact] methods are not allowed to be generic.");
        }
        else
        {
            testCase = this.CreateTestCase(discoveryOptions, testMethod, factAttribute);
        }

        return new[] { testCase };
    }

    protected override IXunitTestCase CreateTestCase(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo factAttribute)
    {
        var category = factAttribute.GetNamedArgument<string?>("Category");
        var tags = factAttribute.GetNamedArgument<string[]?>("Tags");
        var ticketId = factAttribute.GetNamedArgument<string?>("TicketId");
        var longRunning = factAttribute.GetNamedArgument<bool>("LongRunning");
        var traits = new Dictionary<string, List<string?>>();

        if (!category.IsNullOrWhiteSpace())
            traits.Add("tags", category);

        if (tags is { Length: > 0 })
        {
            foreach (var tag in tags)
            {
                if (string.IsNullOrEmpty(tag))
                    continue;

                traits.Add("tags", tag);
            }
        }

        if (longRunning)
            traits.Add("tags", "long-running");

        if (!ticketId.IsNullOrWhiteSpace())
            traits.Add("ticketId", ticketId);

        var attrs = testMethod.Method.GetCustomAttributes(typeof(SkippableTraitAttribute));
        var sb = new StringBuilder();
        foreach (var skippableAttr in attrs)
        {
            if (skippableAttr is not ReflectionAttributeInfo reflect)
            {
                continue;
            }

            if (reflect.Attribute is not SkippableTraitAttribute attr)
                continue;

            var nextReason = attr.GetSkipReason(this.DiagnosticMessageSink, testMethod, factAttribute);
            if (!string.IsNullOrWhiteSpace(nextReason))
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(nextReason);
            }
        }

        var skipReason = sb.ToString();
        var test = new BearzTestCase(
            skipReason,
            traits,
            ServiceProviderLocator,
            this.DiagnosticMessageSink,
            discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(),
            testMethod);

        return test;
    }

    private ExecutionErrorTestCase ErrorTestCase(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        string message) =>
        new(
            this.DiagnosticMessageSink,
            discoveryOptions.MethodDisplayOrDefault(),
            discoveryOptions.MethodDisplayOptionsOrDefault(),
            testMethod,
            message);
}