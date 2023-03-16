using System.CommandLine;
using System.ComponentModel.Composition;

namespace Bearz.Extensions.Hosting.CommandLine;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class SubCommandHandlerAttribute : ExportAttribute
{
    public SubCommandHandlerAttribute(Type commandHandlerType)
        : base(typeof(Command))
    {
        this.CommandHandlerType = commandHandlerType;
    }

    public Type CommandHandlerType { get; }
}