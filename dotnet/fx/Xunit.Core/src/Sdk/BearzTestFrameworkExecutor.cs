using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Xunit.Abstractions;

namespace Xunit.Sdk;

public class BearzTestFrameworkExecutor : XunitTestFrameworkExecutor
{
    public BearzTestFrameworkExecutor(
        AssemblyName assemblyName,
        ISourceInformationProvider sourceInformationProvider,
        IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
        BearzTestDiscoverer.ServiceProviderLocator ??= new TestServiceProviderLocator();
    }

    /// <inheritdoc />
    [SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "Required by xunit api")]
    protected override async void RunTestCases(
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        var locator = BearzTestDiscoverer.ServiceProviderLocator;
        IServiceProvider? serviceProvider = null;
        var exceptionList = new List<Exception>();
        try
        {
            serviceProvider = locator?.GetServiceProvider(this.TestAssembly.Assembly);
        }
        catch (Exception ex)
        {
            exceptionList.Add(ex);
        }

        using var runner = new BearzTestAssemblyRunner(
            serviceProvider,
            this.TestAssembly,
            testCases,
            this.DiagnosticMessageSink,
            executionMessageSink,
            executionOptions,
            exceptionList);

        await runner.RunAsync().ConfigureAwait(false);
    }
}