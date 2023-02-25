using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Xunit.Abstractions;

namespace Xunit.Sdk;

public sealed class BearzTestFramework : XunitTestFramework
{
    public BearzTestFramework(IMessageSink messageSink)
        : base(messageSink)
    {
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
        new BearzTestFrameworkExecutor(assemblyName, this.SourceInformationProvider, this.DiagnosticMessageSink);
}