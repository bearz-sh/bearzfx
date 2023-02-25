#if NETLEGACY

// ReSharper disable once CheckNamespace
namespace System.Runtime.Versioning;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]

// ReSharper disable once InconsistentNaming
public abstract class OSPlatformAttribute : Attribute
{
    protected OSPlatformAttribute(string platformName)
    {
        this.PlatformName = platformName;
    }

    public string PlatformName { get; }
}

#endif