using System.CommandLine;

namespace Bearz.Extensions.Hosting.CommandLine;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class CommandHandlerAttribute : Attribute
{
    public CommandHandlerAttribute(Type commandHandlerType)
    {
        this.CommandHandlerType = commandHandlerType;
    }

    public Type CommandHandlerType { get; }
}