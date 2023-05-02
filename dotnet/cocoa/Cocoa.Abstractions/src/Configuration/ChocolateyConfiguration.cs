using System.Reflection;
using System.Text;

using Bearz.Extra.Strings;

using Cocoa.Configuration.Sections;
using Cocoa.Domain;

using Microsoft.Extensions.Logging;

namespace Cocoa.Configuration;

/// <summary>
///   The chocolatey configuration.
/// </summary>
// On .NET 4.0, get error CS0200 when the setter is private for serialization - see http://stackoverflow.com/a/23809226/18475.
[Serializable]
public class ChocolateyConfiguration
{
    private const int MaxConsoleLineLength = 72;

    private int currentLineLength;

    [NonSerialized]
    private Stack<ChocolateyConfiguration>? configurationBackups;

    public ChocolateyConfiguration()
    {
        this.RegularOutput = true;
        this.PromptForConfirmation = true;
        this.DisableCompatibilityChecks = false;
        this.SourceType = SourceTypes.Normal;
        this.Information = new InformationCommandConfiguration();
        this.Features = new FeaturesConfiguration();
        this.NewCommand = new NewCommandConfiguration();
        this.ListCommand = new ListCommandConfiguration();
        this.UpgradeCommand = new UpgradeCommandConfiguration();
        this.SourceCommand = new SourcesCommandConfiguration();
        this.MachineSources = new List<MachineSourceConfiguration>();
        this.FeatureCommand = new FeatureCommandConfiguration();
        this.ConfigCommand = new ConfigCommandConfiguration();
        this.ApiKeyCommand = new ApiKeyCommandConfiguration();
        this.PackCommand = new PackCommandConfiguration();
        this.PushCommand = new PushCommandConfiguration();
        this.PinCommand = new PinCommandConfiguration();
        this.OutdatedCommand = new OutdatedCommandConfiguration();
        this.Proxy = new ProxyConfiguration();
        this.ExportCommand = new ExportCommandConfiguration();
        this.TemplateCommand = new TemplateCommandConfiguration();
#if DEBUG
        this.AllowUnofficialBuild = true;
#endif
    }

    /// <summary>
    ///   Gets or sets the name of the command.
    ///   This is the command that choco runs.
    /// </summary>
    /// <value>
    ///   The name of the command.
    /// </value>
    public string? CommandName { get; set; }

    // configuration set variables
    public string? CacheLocation { get; set; }

    public int CommandExecutionTimeoutSeconds { get; set; }

    public int WebRequestTimeoutSeconds { get; set; }

    public string? DefaultTemplateName { get; set; }

    /// <summary>
    ///  Gets or sets one or more source locations set by configuration or by command line. Separated by semi-colon.
    /// </summary>
    public string? Sources { get; set; }

    public string? SourceType { get; set; }

    // top level commands
    public bool Debug { get; set; }

    public bool Verbose { get; set; }

    public bool Trace { get; set; }

    public bool Force { get; set; }

    public bool Noop { get; set; }

    public bool HelpRequested { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether parsing was successful (everything parsed) or not.
    /// </summary>
    public bool UnsuccessfulParsing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether output should be limited.
    /// This supports the --limit-output parameter.
    /// </summary>
    /// <value><c>true</c> for regular output; <c>false</c> for limited output.</value>
    // TODO: #2564 Should look into using mutually exclusive output levels - Debug, Info (Regular), Error (Quiet)
    // Verbose and Important are not part of the levels at all
    public bool RegularOutput { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether console logging should be suppressed.
    /// This is for use by API calls which surface results in alternate forms.
    /// </summary>
    /// <value><c>true</c> for no output; <c>false</c> for regular or limited output.</value>
    /// <remarks>This has only been implemented for NuGet List.</remarks>
    public bool QuietOutput { get; set; }

    public bool PromptForConfirmation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether runtime Chocolatey compatibility checks
    /// should be completed or not. Overriding this value is only expected on systems
    /// where the user is explicitly opting out of these checks, for example, when
    /// they are running a perpetual license, where the version is known to not be
    /// compliant, but also that most things "should" work.
    /// </summary>
    public bool DisableCompatibilityChecks { get; set; }

    public bool AcceptLicense { get; set; }

    public bool AllowUnofficialBuild { get; set; }

    public string? AdditionalLogFileLocation { get; set; }

    /// <summary>
    ///  Gets or sets the input. Input is usually related to unparsed arguments.
    /// </summary>
    public string? Input { get; set; }

    // command level options
    public string? Version { get; set; }

    public bool AllVersions { get; set; }

    public bool SkipPackageInstallProvider { get; set; }

    public string? OutputDirectory { get; set; }

    public bool SkipHookScripts { get; set; }

    /// <summary>
    ///   Gets or sets the package names. Space separated.
    /// </summary>
    /// <value>
    ///   Space separated package names.
    /// </value>
    // install/update
    public string? PackageNames { get; set; }

    public bool Prerelease { get; set; }

    public bool ForceX86 { get; set; }

    public string? InstallArguments { get; set; }

    public bool OverrideArguments { get; set; }

    public bool NotSilent { get; set; }

    public string? PackageParameters { get; set; }

    public bool ApplyPackageParametersToDependencies { get; set; }

    public bool ApplyInstallArgumentsToDependencies { get; set; }

    public bool IgnoreDependencies { get; set; }

    public bool AllowDowngrade { get; set; }

    public bool ForceDependencies { get; set; }

    public string? DownloadChecksum { get; set; }

    public string? DownloadChecksum64 { get; set; }

    public string? DownloadChecksumType { get; set; }

    public string? DownloadChecksumType64 { get; set; }

    public bool PinPackage { get; set; }

    /// <summary>
    ///  Gets or sets the configuration values provided by choco.
    /// </summary>
    public InformationCommandConfiguration Information { get; set; }

    /// <summary>
    ///  Gets or sets the configuration related to features and whether they are enabled.
    /// </summary>
    public FeaturesConfiguration Features { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to List command.
    /// </summary>
    public ListCommandConfiguration ListCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to Upgrade command.
    /// </summary>
    public UpgradeCommandConfiguration UpgradeCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to New command.
    /// </summary>
    public NewCommandConfiguration NewCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to Source command.
    /// </summary>
    public SourcesCommandConfiguration SourceCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration for the Default Machine Sources.
    /// </summary>
    public IList<MachineSourceConfiguration> MachineSources { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to the Feature command.
    /// </summary>
    public FeatureCommandConfiguration FeatureCommand { get; set; }

    /// <summary>
    ///  Gets or sets the configuration related to the configuration file.
    /// </summary>
    public ConfigCommandConfiguration ConfigCommand { get; set; }

    /// <summary>
    ///  Gets or sets the configuration related specifically to ApiKey command.
    /// </summary>
    public ApiKeyCommandConfiguration ApiKeyCommand { get; set; }

    /// <summary>
    ///  Gets or sets the configuration related specifically to the Pack command.
    /// </summary>
    public PackCommandConfiguration PackCommand { get; set; }

    /// <summary>
    ///  Gets or sets the configuration related specifically to Push command.
    /// </summary>
    public PushCommandConfiguration PushCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to Pin command.
    /// </summary>
    public PinCommandConfiguration PinCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to Outdated command.
    /// </summary>
    public OutdatedCommandConfiguration OutdatedCommand { get; set; }

    public ExportCommandConfiguration ExportCommand { get; set; }

    /// <summary>
    /// Gets or sets the configuration related specifically to proxies.
    /// </summary>
    public ProxyConfiguration Proxy { get; set; }

    /// <summary>
    ///  Gets or sets the configuration related specifically to Template command.
    /// </summary>
    public TemplateCommandConfiguration TemplateCommand { get; set; }

    /// <summary>
    /// Creates a backup of the current version of the configuration class.
    /// </summary>
    /// <exception cref="System.Runtime.Serialization.SerializationException">One or more objects in the class or child classes are not serializable.</exception>
    public void CreateBackup()
    {
        this.configurationBackups ??= new Stack<ChocolateyConfiguration>();

        // We do this the easy way to ensure that we have a clean copy
        // of the original configuration file.
        this.configurationBackups.Push(this.DeepCopy());
    }

    /// <summary>
    /// Restore the backup that has previously been created to the initial
    /// state, without making the class reference types the same to prevent
    /// the initial configuration class being updated at the same time if a
    /// value changes.
    /// </summary>
    /// <param name="removeBackup">Whether a backup that was previously made should be removed after resetting the configuration.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="InvalidOperationException">No backup has been created before trying to reset the current configuration, and removal of the backup was not requested.</exception>
    /// <remarks>
    /// This call may make quite a lot of allocations on the Gen0 heap, as such
    /// it is best to keep the calls to this method at a minimum.
    /// </remarks>
    public void RevertChanges(bool removeBackup = false, ILogger? logger = null)
    {
        if (this.configurationBackups == null || this.configurationBackups.Count == 0)
        {
            if (!removeBackup)
            {
                throw new InvalidOperationException(
                    "No backup has been created before trying to reset the current configuration, and removal of the backup was not requested.");
            }

            // If we will also be removing the backup, we do not care if it is already
            // null or not, as that is the intended state when this method returns.
            logger?.LogDebug("Requested removal of a configuration backup that does not exist: the backup stack is empty.");
            return;
        }

        // Runtime type lookup ensures this also fully works with derived classes (for example: licensed configuration)
        // without needing to re-implement this method / make it overridable.
        var t = this.GetType();
        var backup = removeBackup ? this.configurationBackups.Pop() : this.configurationBackups.Peek();

        foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.Name == "_configurationBackups")
            {
                continue;
            }

            try
            {
                if (property.Name == "PromptForConfirmation")
                {
                    // We do not overwrite this value between backups as it is intended to be a global setting;
                    // if a user has selected a "[A] yes to all" prompt interactively, this option is
                    // set and should be retained for the duration of the operations.
                    continue;
                }

                var originalValue = property.GetValue(backup, Array.Empty<object>());

                var parentType = property.DeclaringType;
                if (removeBackup || parentType?.IsPrimitive == true || parentType?.IsValueType == true || parentType == typeof(string))
                {
                    // If the property is a primitive, a value type or a string, then a copy of the value
                    // will be created by the .NET Runtime automatically, and we do not need to create a deep clone of the value.
                    // Additionally, if we will be removing the backup there is no need to create a deep copy
                    // for any reference types. We won't have any duplicate references because the backup is being discarded.
                    property.SetValue(this, originalValue, Array.Empty<object>());
                }
                else if (originalValue != null)
                {
                    // We need to do a deep copy of the value so it won't copy the reference itself,
                    // but rather the actual values we are interested in.
                    property.SetValue(this, originalValue.DeepCopy(), Array.Empty<object>());
                }
                else
                {
                    property.SetValue(this, null, Array.Empty<object>());
                }
            }
            catch (Exception ex)
            {
#pragma warning disable S112 // TODO: look for a different exception type instead of ApplicationException
                throw new ApplicationException($"Unable to restore the value for the property '{property.Name}'.", ex);
#pragma warning restore S112
            }
        }
    }

    // overrides
    public override string ToString()
    {
        var properties = new StringBuilder();

        /*
         *  never log in a ToString() method
         *  this.Log().Debug(ChocolateyLoggers.Important, @"
NOTE: Hiding sensitive configuration data! Please double and triple
check to be sure no sensitive data is shown, especially if copying
output to a gist for review.");
         */

        this.OutputToString(properties, this.GetType().GetProperties(), this, string.Empty);
        return properties.ToString();
    }

    private void OutputToString(StringBuilder propertyValues, IEnumerable<PropertyInfo>? properties, object obj, string? prepend)
    {
        properties ??= Array.Empty<PropertyInfo>();
        prepend = prepend.IsNullOrWhiteSpace() ? string.Empty : prepend + ".";
        foreach (var propertyInfo in properties)
        {
            var name = propertyInfo.Name;

            // skip sensitive data info
            if (name is "Key" or "ConfigValue" or "MachineSources" ||
                name.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("sensitive", StringComparison.OrdinalIgnoreCase))
                continue;

            var objectValue = propertyInfo.GetValue(obj, null);
            if (objectValue is null)
            {
                this.AppendOutput(propertyValues, $"{prepend}{name}={{null}}");
            }
            else if (propertyInfo.PropertyType.IsBuiltinType())
            {
                if (!objectValue.ToString().IsNullOrWhiteSpace())
                {
                    this.AppendOutput(propertyValues,  $"{prepend}{name}='{objectValue}'|");
                }
            }
            else if (objectValue is IDictionary<string, string> dictionary)
            {
                foreach (var item in dictionary)
                {
                    this.AppendOutput(propertyValues,  $"{prepend}{name}.{item.Key}='{item.Value}'|");
                }
            }
            else
            {
                this.OutputToString(propertyValues, propertyInfo.PropertyType.GetProperties(), objectValue, propertyInfo.Name);
            }
        }
    }

    private void AppendOutput(StringBuilder propertyValues, string append)
    {
        this.currentLineLength += append.Length;
        propertyValues
            .Append(this.currentLineLength < MaxConsoleLineLength ? string.Empty : Environment.NewLine)
            .Append(append)
            .Append(append.Length < MaxConsoleLineLength ? string.Empty : Environment.NewLine);

        if (this.currentLineLength > MaxConsoleLineLength)
            this.currentLineLength = append.Length;
    }
}