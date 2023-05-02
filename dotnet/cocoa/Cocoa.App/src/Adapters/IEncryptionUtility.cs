namespace Cocoa.Adapters;

public interface IEncryptionUtility
{
    string EncryptString(string cleartextValue);

    string DecryptString(string encryptedString);

    string GenerateUniqueToken(string caseInsensitiveKey);
}