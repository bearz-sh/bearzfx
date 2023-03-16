namespace Bearz.Security.Cryptography
{
    /// <summary>
    /// The type of action for the delegate <see cref="Bearz.Security.Cryptography.MemoryProtectionAction"/>.
    /// </summary>
    public enum MemoryProtectionActionType
    {
        /// <summary>Encrypt data.</summary>
        Encrypt,

        /// <summary>Decrypt data.</summary>
        Decrypt,
    }
}