using System.Buffers;
using System.Security.Cryptography;

using Bearz.Security.Cryptography;

using static Bearz.KeePass.Cryptography.KeePassIdGenerator;

namespace Bearz.KeePass.Cryptography;


/// <summary>
/// Encrypts binary data that is stored in memory.
/// </summary>
/// <remarks>
///     <para>
///         https://msdn.microsoft.com/en-us/library/system.security.cryptography.protectedmemory.aspx.
///     </para>
/// </remarks>
public sealed class ProtectedBuffer : IEquatable<ProtectedBuffer>
{
    private readonly byte[] binary;
    private readonly int hashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtectedBuffer"/> class.
    /// </summary>
    public ProtectedBuffer()
    {
        this.Id = GenerateId();
        this.Length = 0;
        this.binary = Array.Empty<byte>();
        this.hashCode = HashCode.Combine(this.Id, this.binary);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtectedBuffer"/> class.
    /// </summary>
    /// <param name="binary">The binary data that will be protected.</param>
    /// <param name="encrypt">should the binary data be encrypted or not.</param>
    public ProtectedBuffer(byte[] binary, bool encrypt = true)
        : this()
    {
        this.Length = binary.Length;
        this.IsProtected = encrypt;
        this.Id = GenerateId();

        this.hashCode = HashCode.Combine(this.binary, this.Id);

        if (!encrypt)
        {
            this.binary = binary;
            return;
        }

        // from the MSDN Docs
        // userData must be 16 bytes in length or in multiples of 16 bytes.
        // https://msdn.microsoft.com/en-us/library/system.security.cryptography.protectedmemory.protect.aspx
        binary = Grow(binary, 16);

        this.binary = new byte[binary.Length];
        this.Encrypt(binary).CopyTo(binary);
    }

    public static ProtectedBuffer Empty { get; } = new();

    /// <summary>
    /// Gets the unique id for this object.
    /// </summary>
    public byte[] Id { get; }

    /// <summary>
    /// Gets the length of the binary data.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets a value indicating whether is the data protected.
    /// </summary>
    public bool IsProtected { get; }

    public bool IsEmpty => this.Length == 0;

    /// <summary>
    /// Gets a hash code for this object.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return this.hashCode;
    }

    /// <summary>
    /// Decrypts the inner data and returns a copy.
    /// </summary>
    /// <returns>Returns a copy of the data.</returns>
    public ReadOnlySpan<byte> Unprotect()
    {
        if (!this.IsProtected)
            return this.binary;

        return this.Decrypt();
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj is ProtectedBuffer protectedBuffer)
            return this.Equals(protectedBuffer);

        return false;
    }

    /// <summary>
    /// Determines if the given object is equal to the current instance.
    /// </summary>
    /// <param name="other">That object to compare.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public bool Equals(ProtectedBuffer? other)
        => this.Equals(other, true);

    public bool Equals(ProtectedBuffer? other, bool onlyWhenProtected)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (onlyWhenProtected && this.IsProtected != other.IsProtected)
            return false;

        if (this.Length != other.Length)
            return false;

        return this.Decrypt().SequenceEqual(other.Decrypt());
    }

    public override string ToString()
    {
        return "***********";
    }

    public void Clear()
    {
        Array.Clear(this.binary, 0, this.binary.Length);
    }

    internal static int GetNextSize(int length, int blockSize)
    {
        var blocks = length / blockSize;
        int size = blocks * blockSize;
        if (size > length)
            return size;

        while (size < length)
        {
            blocks++;
            size = blocks * blockSize;
        }

        return size;
    }

    internal static byte[] Grow(byte[] binary, int blockSize)
    {
        int length = binary.Length;
        int blocks = binary.Length / blockSize;
        int size = blocks * blockSize;
        if (size <= length)
        {
            while (size < length)
            {
                blocks++;
                size = blocks * blockSize;
            }
        }

        byte[] result = new byte[blocks * blockSize];
        Array.Copy(binary, result, binary.Length);
        return result;
    }

    private ReadOnlySpan<byte> Encrypt(ReadOnlySpan<byte> bytes)
    {
        if (!this.IsProtected)
            return this.binary;

        var action = ProtectionOptions.DataProtectionAction;
        return action(bytes, this, DataProtectionActionType.Encrypt);
    }

    private ReadOnlySpan<byte> Decrypt()
    {
        if (!this.IsProtected)
            return this.binary;

        var action = ProtectionOptions.DataProtectionAction;
        return action(this.binary, this, DataProtectionActionType.Decrypt);
    }
}