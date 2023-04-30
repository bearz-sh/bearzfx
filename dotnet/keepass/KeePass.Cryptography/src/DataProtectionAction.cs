namespace Bearz.KeePass.Cryptography;

/// <summary>
/// A delegate for encrypting or decrypting binary data, so that encryption methods can
/// be swapped out.
/// </summary>
/// <param name="binary">The binary data that will be encrypted or decrypted.</param>
/// <param name="state">state that helps with the encryption / decryption process.</param>
/// <param name="action">The type of action for the delegate to perform.</param>
/// <returns>Binary data that is encrypted or decrypted. </returns>
public delegate ReadOnlySpan<byte> DataProtectionAction(ReadOnlySpan<byte> binary, object state, DataProtectionActionType action);

/// <summary>
/// The type of action for the delegate <see cref="DataProtectionAction"/>.
/// </summary>
public enum DataProtectionActionType
{
    Encrypt,
    Decrypt,
}