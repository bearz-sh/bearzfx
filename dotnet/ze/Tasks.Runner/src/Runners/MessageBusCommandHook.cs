using Bearz.Cli;
using Bearz.Secrets;

using Ze.Tasks.Messages;

namespace Ze.Tasks.Runner;

public class MessageBusCommandHook : IPreCliCommandHook
{
    public MessageBusCommandHook(IMessageBus messageBus, ISecretMasker masker)
    {
        this.MessageBus = messageBus;
        this.Masker = masker;
    }

    private IMessageBus MessageBus { get; }

    private ISecretMasker Masker { get; }

    public void Next(CliCommand command)
    {
        var exe = command.FileName;
        var args = this.Masker.Mask(command.StartInfo.Args.ToString());

        this.MessageBus.Publish(new LogMessage($"{exe} {args}", LogLevel.Command));
    }
}