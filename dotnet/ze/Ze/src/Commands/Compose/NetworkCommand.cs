using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;
using Bearz.Std;

using Command = System.CommandLine.Command;

namespace Ze.Commands.Compose;

[CommandHandler(typeof(NetworkCommandHandler))]
public class NetworkCommand : Command
{
    public NetworkCommand()
        : base("network", "manages plank compose networks")
    {
        this.AddCommand(new NetworkCreateCommand());
        this.AddCommand(new NetworkDeleteCommand());
    }
}

public class NetworkCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        context.HelpBuilder.Write(new HelpContext(context.HelpBuilder, context.BindingContext.ParseResult.CommandResult.Command, Console.Out));
        return Task.FromResult(0);
    }
}

[CommandHandler(typeof(NetworkDeleteCommandHandler))]
public class NetworkDeleteCommand : Command
{
    public NetworkDeleteCommand()
        : base("delete", "Removes one or more networks  ")
    {
        this.AddArgument(new Argument<string[]>("network", "Network to remove"));
        this.AddOption(new Option<bool>(new[] { "--force", "-f" }, "Do not error if the network does not exist"));
    }
}

public class NetworkDeleteCommandHandler : ICommandHandler
{
    public string[] Network { get; set; } = Array.Empty<string>();

    public bool Force { get; set; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var args = new CommandArgs()
        {
            "network",
            "rm",
        };

        if (this.Network.Length == 0)
        {
            Console.WriteLine("No networks specified");
            return 1;
        }

        if (this.Force)
            args.Add("--force");

        args.AddRange(this.Network);

        var cmd = await Env.Process.CreateCommand("docker")
            .WithArgs(args)
            .WithStdio(Stdio.Inherit)
            .OutputAsync(context.GetCancellationToken())
            .ConfigureAwait(false);

        return cmd.ExitCode;
    }
}

[CommandHandler(typeof(NetworkCreateCommandHandler))]
public class NetworkCreateCommand : Command
{
    public NetworkCreateCommand()
        : base("create", "creates a plank compose network")
    {
        this.AddArgument(new Argument<string>("name", () => "plank-vnet", "the name of the network"));
        this.AddOption(new Option<string>(new[] { "--driver", "-d" }, () => "bridge", "the driver to use"));
        this.AddOption(new Option<string>("--range", () => "172.23.0.0", "the driver to use"));
        this.AddOption(new Option<string[]>("--subnet", "Subnet in CIDR format that represents a network segment"));
        this.AddOption(new Option<string?>("--gateway", "IPv4 or IPv6 Gateway for the master subnet"));
        this.AddOption(new Option<string[]>("--aux-address", "Auxiliary IPv4 or IPv6 addresses used by Network driver"));
        this.AddOption(new Option<bool>("--internal", "Restrict external access to the network"));
        this.AddOption(new Option<bool>("--ipv6", "Enable IPv6 networking"));
        this.AddOption(new Option<bool>("--ingress", "Create swarm routing-mesh network"));
        this.AddOption(new Option<bool>("--attachable", "Enable manual container attachment"));
        this.AddOption(new Option<string>("--scope", "Control the network's scope"));
        this.AddOption(new Option<string>("--config-from", "The network from which to copy the configuration"));
        this.AddOption(new Option<bool>("--config-only", "Create a configuration only network"));
        this.AddOption(new Option<string>("--ipam-driver", () => "default", "IP Address Management Driver"));
        this.AddOption(new Option<string[]>("--ipam-opt", "Set IPAM driver specific options"));
        this.AddOption(new Option<string[]>("--label", "Set metadata on a network"));
        this.AddOption(new Option<string[]>("--opt", "Set driver specific options"));
    }
}

public class NetworkCreateCommandHandler : ICommandHandler
{
    public string? Name { get; set; }

    public string? Driver { get; set; }

    public string? Range { get; set; }

    public string[]? Subnet { get; set; }

    public string? Gateway { get; set; }

    public string[]? AuxAddress { get; set; }

    public bool? Internal { get; set; }

    public bool? Ipv6 { get; set; }

    public bool? Ingress { get; set; }

    public bool? Attachable { get; set; }

    public string? Scope { get; set; }

    public string? ConfigFrom { get; set; }

    public bool? ConfigOnly { get; set; }

    public string? IpamDriver { get; set; }

    public string[]? IpamOpt { get; set; }

    public string[]? Label { get; set; }

    public string[]? Opt { get; set; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var args = new CommandArgs()
        {
            "network",
            "create",
        };

        if (!this.Range.IsNullOrWhiteSpace())
        {
            args.Add("--subnet", $"{this.Range}/21");
            args.Add("--gateway", $"{this.Range.Substring(0, this.Range.LastIndexOf('.'))}.1");
        }
        else
        {
            if (this.Subnet is { Length: > 0 })
            {
                foreach (var subnet in this.Subnet)
                {
                    args.Add("--subnet", subnet);
                }
            }

            if (!this.Gateway.IsNullOrWhiteSpace())
            {
                args.Add("--gateway", this.Gateway);
            }
        }

        if (this.AuxAddress is { Length: > 0 })
        {
            foreach (var auxAddress in this.AuxAddress)
            {
                args.Add("--aux-address", auxAddress);
            }
        }

        if (this.Internal is { })
        {
            args.Add("--internal");
        }

        if (this.Ipv6 is { })
        {
            args.Add("--ipv6");
        }

        if (this.Ingress is { })
        {
            args.Add("--ingress");
        }

        if (this.Attachable is { })
        {
            args.Add("--attachable");
        }

        if (!this.Scope.IsNullOrWhiteSpace())
        {
            args.Add("--scope", this.Scope);
        }

        if (!this.ConfigFrom.IsNullOrWhiteSpace())
        {
            args.Add("--config-from", this.ConfigFrom);
        }

        if (this.ConfigOnly is { })
        {
            args.Add("--config-only");
        }

        if (!this.IpamDriver.IsNullOrWhiteSpace())
        {
            args.Add("--ipam-driver", this.IpamDriver);
        }

        if (this.IpamOpt is { Length: > 0 })
        {
            foreach (var ipamOpt in this.IpamOpt)
            {
                args.Add("--ipam-opt", ipamOpt);
            }
        }

        if (this.Label is { Length: > 0 })
        {
            foreach (var label in this.Label)
            {
                args.Add("--label", label);
            }
        }

        if (this.Opt is { Length: > 0 })
        {
            foreach (var opt in this.Opt)
            {
                args.Add("--opt", opt);
            }
        }

        args.Add(this.Name);

        var result = await Env.Process.CreateCommand("docker")
            .WithArgs(args)
            .WithStdio(Stdio.Inherit)
            .OutputAsync(context.GetCancellationToken())
            .ConfigureAwait(false);

        return result.ExitCode;
    }
}