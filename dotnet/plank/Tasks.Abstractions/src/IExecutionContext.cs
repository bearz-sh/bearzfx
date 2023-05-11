using Bearz.Cli;
using Bearz.Virtual;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Plank.Tasks;

public interface IExecutionContext : ICliExecutionContext
{
    ILogger Log { get; }

    IConfiguration Config { get; }

    IMessageBus Bus { get; }

    IVariables Variables { get; }
}