namespace Bearz.Std;

public enum EnvFolderOption
{
    /// <summary>The path to the folder is verified. If the folder exists, the path is returned. If the folder does not exist, an empty string is returned. This is the default behavior.</summary>
    None = 0,

    /// <summary>The path to the folder is returned without verifying whether the path exists. If the folder is located on a network, specifying this option can reduce lag time.</summary>
    DoNotVerify = 16384, // 0x00004000

    /// <summary>The path to the folder is created if it does not already exist.</summary>
    Create = 32768, // 0x00008000
}