using Microsoft.Win32;

namespace Cocoa.Registry;

public class GenericRegistryKey
{
    public string Name { get; set; } = string.Empty;

    public IEnumerable<GenericRegistryKey> Keys { get; set; } = Array.Empty<GenericRegistryKey>();

    public IEnumerable<GenericRegistryValue> Values { get; set; } = Array.Empty<GenericRegistryValue>();

    public RegistryView View { get; set; }
}