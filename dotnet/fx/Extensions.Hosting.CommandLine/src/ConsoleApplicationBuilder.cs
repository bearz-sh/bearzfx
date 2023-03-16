using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Parser = System.CommandLine.Parsing.Parser;
using ParseResult = System.CommandLine.Parsing.ParseResult;

namespace Bearz.Extensions.Hosting.CommandLine;

public class ConsoleApplicationBuilder
{
    private readonly CommandLineBuilder builder;

    private readonly HostApplicationBuilder appBuilder;

    private readonly Dictionary<Type, Type> commandHandlers = new();

    public ConsoleApplicationBuilder()
    {
        this.RootCommand = new RootCommand();
        this.builder = new CommandLineBuilder(this.RootCommand);
        this.appBuilder = new HostApplicationBuilder();

        this.Services.AddSingleton<IHostLifetime, InvocationLifetime>();
    }

    public RootCommand RootCommand { get; }

    public IServiceCollection Services => this.appBuilder.Services;

    public ConfigurationManager Configuration => this.appBuilder.Configuration;

    public IHostEnvironment Environment => this.appBuilder.Environment;

    public ILoggingBuilder Logging => this.appBuilder.Logging;

    [ImportMany(typeof(Command))]
    public IEnumerable<Command> ImportedCommands { get; set; } = Array.Empty<Command>();

    public ConsoleApplicationBuilder AddMiddleware(Action<InvocationContext> onInvoke)
        => this.AddMiddleware(onInvoke, MiddlewareOrder.Default);

    public ConsoleApplicationBuilder AddMiddleware(Action<InvocationContext> onInvoke, MiddlewareOrder order)
    {
        this.builder.AddMiddleware(onInvoke, order);

        return this;
    }

    public ConsoleApplicationBuilder AddMiddleware(InvocationMiddleware middleware)
        => this.AddMiddleware(middleware, MiddlewareOrder.Default);

    public ConsoleApplicationBuilder AddMiddleware(InvocationMiddleware middleware, MiddlewareOrder order)
    {
        this.builder.AddMiddleware(middleware, order);

        return this;
    }

    public ConsoleApplicationBuilder UseComposition(Action<AggregateCatalog> configureCatalog)
    {
        var catalog = new AggregateCatalog();
        configureCatalog(catalog);
        var container = new CompositionContainer(catalog);
        container.ComposeParts(this);

        foreach (var cmd in this.ImportedCommands)
        {
            this.RootCommand.Add(cmd);

            AddCommandHandler(this, cmd);
        }

        return this;
    }

    public ConsoleApplicationBuilder UseDefaults()
    {
        this.builder.UseDefaults();

        return this;
    }

    public ConsoleApplicationBuilder UseVersionOption()
    {
        this.builder.UseVersionOption();

        return this;
    }

    public ConsoleApplicationBuilder UseVersionOption(string[] aliases)
    {
        this.builder.UseVersionOption(aliases);

        return this;
    }

    public ConsoleApplicationBuilder UseTypeCorrections(int maxLevensteinDistance = 3)
    {
        this.builder.UseTypoCorrections(maxLevensteinDistance);

        return this;
    }

    public ConsoleApplicationBuilder UseExceptionHandler(
        Action<Exception, InvocationContext> onException,
        int? errorExitCode = null)
    {
        this.builder.UseExceptionHandler(onException, errorExitCode);

        return this;
    }

    public ConsoleApplicationBuilder UseSuggestDirective()
    {
        this.builder.UseSuggestDirective();

        return this;
    }

    public ConsoleApplicationBuilder AddCommand(Command command)
    {
        this.RootCommand.Add(command);
        AddCommandHandler(this, command);

        return this;
    }

    public ConsoleApplicationBuilder AddCommand<TCommand>()
        where TCommand : Command, new()
    {
        this.AddCommand(new TCommand());
        return this;
    }

    public ConsoleApplicationBuilder AddCommand(Command command, Type commandHandlerType)
    {
        AddCommandHandler(this, command);
        this.UseCommandHandler(command.GetType(), commandHandlerType);
        return this;
    }

    public ConsoleApplicationBuilder AddCommand<TCommand>(Type commandHandlerType)
        where TCommand : Command, new()
    {
        this.AddCommand(new TCommand());
        this.UseCommandHandler(typeof(TCommand), commandHandlerType);
        return this;
    }

    public ConsoleApplicationBuilder UseCommandHandler(Type commandType, Type commandHandlerType)
    {
        this.commandHandlers.Add(commandType, commandHandlerType);
        this.Services.AddTransient(commandHandlerType);

        return this;
    }

    public ConsoleApplicationBuilder UseCommandHandler<TCommand, THandler>()
        where TCommand : Command
        where THandler : ICommandHandler
        => this.UseCommandHandler(typeof(TCommand), typeof(THandler));

    public Parser Build()
    {
        this.builder.AddMiddleware(async (invocation, next) =>
        {
            var argsRemaining = invocation.ParseResult.UnparsedTokens.ToArray();

            this.Configuration.AddCommandLine(argsRemaining);
            this.Configuration.AddCommandLineDirectives(invocation.ParseResult, "config");

            this.Services.AddSingleton(invocation);
            this.Services.AddSingleton(invocation.BindingContext);
            this.Services.AddSingleton(invocation.Console);
            this.Services.AddTransient(typeof(IInvocationResult), _ => invocation.InvocationResult!);
            this.Services.AddTransient(typeof(ParseResult), _ => invocation.ParseResult);

            foreach (var kv in this.commandHandlers)
            {
                var commandType = kv.Key;
                var handlerType = kv.Value;
                var cmd = invocation.ParseResult.CommandResult.Command;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (cmd is not null
                    && cmd.GetType() == commandType)
                {
                    invocation.BindingContext.AddService(
                        handlerType,
                        c => c.GetRequiredService<IHost>().Services.GetService(handlerType)!);

                    var handler = handlerType.GetMethod(nameof(ICommandHandler.InvokeAsync));
                    if (handler is not null)
                        cmd.Handler = CommandHandler.Create(handler);
                }
            }

            var host = this.appBuilder.Build();

            invocation.BindingContext.AddService(typeof(IHost), _ => host);

            await host.StartAsync();

            await next(invocation);

            await host.StopAsync();
        });

        return this.builder.Build();
    }

    private static void AddCommandHandler(ConsoleApplicationBuilder builder, Command command)
    {
        if (command.Handler == null)
        {
            var type = command.GetType();
            var attr = type.GetCustomAttribute<CommandHandlerAttribute>();
            if (attr is not null)
            {
                builder.UseCommandHandler(type, attr.CommandHandlerType);
            }
            else
            {
                var subAttr = type.GetCustomAttribute<SubCommandHandlerAttribute>();
                if (subAttr is not null)
                {
                    builder.UseCommandHandler(type, subAttr.CommandHandlerType);
                }
            }
        }

        foreach (var cmd in command.Subcommands)
        {
            AddCommandHandler(builder, cmd);
        }
    }
}