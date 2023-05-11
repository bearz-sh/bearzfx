using Bearz.Cli;
using Bearz.Secrets;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Ze.Tasks.Messages;

namespace Ze.Tasks.Runner.Runners;

public static class RunnerServiceCollectionExtensions
{
    public static IServiceCollection AddRunnerCore(this IServiceCollection services, bool stopOnFail = false)
    {
        services.TryAddSingleton<IMessageSink, LoggerMessageSink>();
        services.TryAddSingleton<ISecretMasker>(_ => SecretMasker.Default);
        services.TryAddScoped<IMessageBus>(s => new MessageBus(s.GetRequiredService<IMessageSink>(), stopOnFail));
        services.TryAddScoped<IPreCliCommandHook>(s =>
            new MessageBusCommandHook(
                s.GetRequiredService<IMessageBus>(),
                s.GetRequiredService<ISecretMasker>()));

        return services;
    }
}