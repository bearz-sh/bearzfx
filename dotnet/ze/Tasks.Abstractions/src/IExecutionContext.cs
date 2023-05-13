using Bearz.Cli;
using Bearz.Secrets;
using Bearz.Virtual;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ze.Tasks;

public interface IExecutionContext : ICliExecutionContext
{
    ILogger Log { get; }

    IConfiguration Config { get; }

    IMessageBus Bus { get; }

    IVariables Variables { get; }

    ISecretMasker SecretMasker { get; }
}