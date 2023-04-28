using System.Management.Automation;
using System.Reflection;

using Bearz.Std;

namespace Bearz.PowerShell;

public static class PsCmdletExtensions
{
    private static bool s_backwardsCompatibilityLayerRegistered;

    public static CommandInfo GetCommand(this PSCmdlet cmdlet, string command)
    {
        return cmdlet.InvokeCommand.GetCommand(command, CommandTypes.All);
    }

    public static CommandInfo GetCommand(this PSCmdlet cmdlet, string command, CommandTypes commandTypes)
    {
        return cmdlet.InvokeCommand.GetCommand(command, commandTypes);
    }

    public static CmdletInfo GetCmdlet(this PSCmdlet cmdlet, string command)
    {
        return cmdlet.InvokeCommand.GetCmdlet(command);
    }

    public static ActionPreference GetCommandPreference(this PSCmdlet cmdlet)
    {
        var globalPreference = cmdlet.GetGlobalVariable("CommandPreference")?.Value;
        if (globalPreference is null)
            return ActionPreference.Continue;

        if (globalPreference is ActionPreference globalActionPreference)
            return globalActionPreference;
        if (globalPreference is string globalActionPreferenceString &&
            Enum.TryParse<ActionPreference>(globalActionPreferenceString, out var actionPreferenceEnum))
            return actionPreferenceEnum;

        return ActionPreference.Continue;
    }

    public static PSVariable? GetVariable(this PSCmdlet cmdlet, string name)
    {
        return cmdlet.SessionState.PSVariable.Get(name);
    }

    public static PSVariable? GetGlobalVariable(this PSCmdlet cmdlet, string name)
        => GetVariable(cmdlet, $"Global:{name}");

    public static PSVariable? GetPrivateVariable(this PSCmdlet cmdlet, string name)
        => GetVariable(cmdlet, $"Private:{name}");

    public static PSVariable? GetScriptVariable(this PSCmdlet cmdlet, string name)
        => GetVariable(cmdlet, $"Script:{name}");

    public static void SetVariable(this PSCmdlet cmdlet, string name, object? value)
    {
        cmdlet.SessionState.PSVariable.Set(name, value);
    }

    public static void SetVariable(this PSCmdlet cmdlet, PSVariable variable)
    {
        cmdlet.SessionState.PSVariable.Set(variable);
    }

    public static Dictionary<string, object?> GetBoundParameters(
        this PSCmdlet cmdlet,
        Dictionary<string, object?> boundParameters)
        => cmdlet.MyInvocation.BoundParameters;

    public static PSCmdlet WriteError(this PSCmdlet cmdlet, Exception exception)
    {
        var errorRecord = new ErrorRecord(exception, exception.GetType().Name, ErrorCategory.NotSpecified, null);
        cmdlet.WriteError(errorRecord);
        return cmdlet;
    }

    public static PSCmdlet WriteError(this PSCmdlet cmdlet, Exception exception, object? targetObject)
    {
        var errorRecord = new ErrorRecord(exception, exception.GetType().Name, ErrorCategory.NotSpecified, null);
        cmdlet.WriteError(errorRecord);
        return cmdlet;
    }

    public static PSCmdlet WriteError(
        this PSCmdlet cmdlet,
        Exception exception,
        string errorId,
        ErrorCategory errorCategory = ErrorCategory.NotSpecified,
        object? targetObject = null)
    {
        var errorRecord = new ErrorRecord(exception, errorId, errorCategory, null);
        cmdlet.WriteError(errorRecord);
        return cmdlet;
    }

    public static PSCmdlet ThrowTerminatingError(this PSCmdlet cmdlet, Exception exception)
    {
        var errorRecord = new ErrorRecord(exception, exception.GetType().Name, ErrorCategory.NotSpecified, null);
        cmdlet.ThrowTerminatingError(errorRecord);
        return cmdlet;
    }

    public static PSCmdlet ThrowTerminatingError(this PSCmdlet cmdlet, Exception exception, object? targetObject)
    {
        var errorRecord = new ErrorRecord(exception, exception.GetType().Name, ErrorCategory.NotSpecified, null);
        cmdlet.ThrowTerminatingError(errorRecord);
        return cmdlet;
    }

    public static PSCmdlet ThrowTerminatingError(
        this PSCmdlet cmdlet,
        Exception exception,
        string errorId,
        ErrorCategory errorCategory = ErrorCategory.NotSpecified,
        object? targetObject = null)
    {
        var errorRecord = new ErrorRecord(exception, errorId, errorCategory, null);
        cmdlet.ThrowTerminatingError(errorRecord);
        return cmdlet;
    }

    public static PSCmdlet WriteCommand(
        this PSCmdlet cmdlet,
        string command,
        CommandArgs? args = null,
        ActionPreference? actionPreference = null)
    {
        if (actionPreference is null)
            actionPreference = cmdlet.GetCommandPreference();

        switch (actionPreference)
        {
            case ActionPreference.Continue:
                var (message, color) = CommandFormatter.FormatMessage(command, args);
                if (!color)
                {
                    cmdlet.Host.UI.WriteLine(message);
                }
                else
                {
                    cmdlet.Host.UI.WriteLine(ConsoleColor.Cyan, cmdlet.Host.UI.RawUI.BackgroundColor, message);
                }

                break;

            default:
                break;
        }

        return cmdlet;
    }

    public static PSCmdlet RegisterBackwardsCompatibility(this PSCmdlet cmdlet)
    {
        if (s_backwardsCompatibilityLayerRegistered)
            return cmdlet;

        s_backwardsCompatibilityLayerRegistered = true;
        var hasIsWindows = cmdlet.GetGlobalVariable("IsWindows") != null;
        var hasIsLinux = cmdlet.GetGlobalVariable("IsLinux") != null;
        var hasIsMacOS = cmdlet.GetGlobalVariable("IsMacOS") != null;
        var isCoreClr = cmdlet.GetGlobalVariable("IsCoreClr") != null;
        var hasProcess64Bit = cmdlet.GetGlobalVariable("IsProcess64Bit") != null;
        var hasOs64Bit = cmdlet.GetGlobalVariable("IsOs64Bit") != null;
        var hasCommandActionPreference = cmdlet.GetGlobalVariable("CommandActionPreference") != null;

        if (!hasIsWindows)
            cmdlet.SetVariable(new PSVariable("Global:IsWindows", Env.IsWindows(), ScopedItemOptions.Constant));

        if (!hasIsLinux)
            cmdlet.SetVariable(new PSVariable("Global:IsLinux", Env.IsLinux(), ScopedItemOptions.Constant));

        if (!hasIsMacOS)
            cmdlet.SetVariable(new PSVariable("Global:IsMacOs", Env.IsMacOS(), ScopedItemOptions.Constant));

        if (!isCoreClr)
            cmdlet.SetVariable(new PSVariable("Global:IsCoreCLR", !hasIsWindows, ScopedItemOptions.Constant));

        if (!hasCommandActionPreference)
        {
            cmdlet.SetVariable(
                new PSVariable(
                    "Global:CommandActionPreference",
                    ActionPreference.Continue,
                    ScopedItemOptions.Constant));
        }

        if (!hasProcess64Bit)
            cmdlet.SetVariable(new PSVariable("Global:IsProcess64Bit", Env.IsProcess64Bit, ScopedItemOptions.Constant));

        if (!hasOs64Bit)
            cmdlet.SetVariable(new PSVariable("Global:IsOs64Bit", Env.IsOs64Bit, ScopedItemOptions.Constant));

#if NETLEGACY
        var type = typeof(PSObject).Assembly.GetType("System.Management.Automation.TypeAccelerators");
        if (type is not null)
        {
#pragma warning disable S3011
            var userTypeAccelerators =
                type.GetField("userTypeAccelerators", BindingFlags.NonPublic | BindingFlags.Static);
#pragma warning restore S3011

            if (userTypeAccelerators != null &&
                userTypeAccelerators.GetValue(null) is Dictionary<string, Type> dictionary &&
                !dictionary.ContainsKey("semver"))
            {
                type.GetMethod(
                        "Add",
                        BindingFlags.Public | BindingFlags.Static)
                    ?.Invoke(null, new object[] { "semver", typeof(SemanticVersion) });
            }
        }
#endif

        return cmdlet;
    }
}