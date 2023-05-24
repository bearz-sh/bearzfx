using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Bearz.Extra.Strings;
using Bearz.Text;

namespace Bearz.KeePass.Cryptography;

[SuppressMessage("ReSharper", "ParameterHidesMember")]
public sealed class ProtectedString : IEquatable<ProtectedString>
{
    private byte[] binary;
    private string? value;
    private int byteLength;

    public ProtectedString()
    {
        this.IsProtected = true;
        this.Id = KeePassIdGenerator.GenerateId();
        this.Length = 0;
        this.binary = Array.Empty<byte>();
    }

    public ProtectedString(byte[] binary, bool isProtected)
    {
        this.IsProtected = isProtected;

        this.Id = KeePassIdGenerator.GenerateId();

        this.byteLength = binary.Length;
        if (!isProtected)
        {
            this.binary = new byte[binary.Length];
            binary.AsSpan().CopyTo(this.binary);
            this.Length = Encodings.Utf8NoBom.GetCharCount(this.binary);
            return;
        }

        var max = ProtectedBuffer.GetNextSize(binary.Length, 16);
        this.binary = new byte[max];
        this.Encrypt(binary).CopyTo(this.binary);
        this.Length = Encodings.Utf8NoBom.GetCharCount(this.binary);
    }

    public ProtectedString(ReadOnlySpan<byte> binary, bool isProtected)
    {
        this.IsProtected = isProtected;
        this.Id = KeePassIdGenerator.GenerateId();

        this.byteLength = binary.Length;
        if (!isProtected)
        {
            this.binary = binary.ToArray();
            this.Length = Encodings.Utf8NoBom.GetCharCount(this.binary);
            return;
        }

        var max = ProtectedBuffer.GetNextSize(binary.Length, 16);
        var copy = new byte[max];
        this.Encrypt(binary).CopyTo(copy);
        this.binary = copy;
        this.Length = Encodings.Utf8NoBom.GetCharCount(this.binary);
    }

    public ProtectedString(string value, bool isProtected)
    {
        this.IsProtected = isProtected;
        this.value = value;
        this.Length = value.Length;
        this.Id = KeePassIdGenerator.GenerateId();
        this.byteLength = Encodings.Utf8NoBom.GetByteCount(value);
        this.binary = Array.Empty<byte>();
    }

    public ProtectedString(ReadOnlySpan<char> value, bool isProtected)
    {
        this.Length = value.Length;
        this.Id = KeePassIdGenerator.GenerateId();
#if !NETLEGACY
        this.byteLength = Encodings.Utf8NoBom.GetByteCount(value);
        if (!isProtected)
        {
            this.binary = new byte[this.byteLength];
            Encodings.Utf8NoBom.GetBytes(value, this.binary);
            this.IsProtected = false;
            this.Id = KeePassIdGenerator.GenerateId();
            return;
        }

        var max = ProtectedBuffer.GetNextSize(this.byteLength, 16);
        this.binary = new byte[max];
        Encodings.Utf8NoBom.GetBytes(value, this.binary);
        this.Encrypt(this.binary).CopyTo(this.binary);
#else
        var chars = ArrayPool<char>.Shared.Rent(value.Length);
        if (!isProtected)
        {
            this.binary = Encodings.Utf8NoBom.GetBytes(chars, 0, value.Length);
            ArrayPool<char>.Shared.Return(chars, true);
            this.IsProtected = false;
            this.Id = KeePassIdGenerator.GenerateId();
            return;
        }

        this.byteLength = Encodings.Utf8NoBom.GetByteCount(chars);
        var max = ProtectedBuffer.GetNextSize(this.byteLength, 16);
        this.binary = new byte[max];
        Encodings.Utf8NoBom.GetBytes(chars, 0, value.Length, this.binary, 0);
        ArrayPool<char>.Shared.Return(chars, true);
        this.Encrypt(this.binary).CopyTo(this.binary);
#endif
    }

    public ProtectedString(StringBuilder builder, bool isProtected)
        : this(builder.AsSpan(), isProtected)
    {
    }

    public static ProtectedString Empty { get; } = new();

    /// <summary>
    /// Gets the unique id for this object.
    /// </summary>
    public byte[] Id { get; }

    /// <summary>
    /// Gets the length of the binary data.
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    /// Gets a value indicating whether is the data protected.
    /// </summary>
    public bool IsProtected { get; }

    public bool IsEmpty => this.Length == 0;

    public static bool operator ==(ProtectedString? left, ProtectedString? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ProtectedString? left, ProtectedString? right)
    {
        return !Equals(left, right);
    }

    public ProtectedString Append(ReadOnlySpan<char> value)
    {
        var l = this.Length + value.Length;
        if (this.value is not null)
        {
            var next = new char[l];
            this.Length = next.Length;
            var span = next.AsSpan();
            this.value.AsSpan().CopyTo(span);
            value.CopyTo(span.Slice(this.value.Length));
            this.value = new string(next);
            this.Length = this.value.Length;
        }

#if !NETLEGACY
        var bl = this.byteLength;
        this.byteLength = this.byteLength + Encodings.Utf8NoBom.GetByteCount(value);
        var copy = new byte[this.byteLength];
        this.Decrypt().Slice(0, bl).CopyTo(copy);
        Encodings.Utf8NoBom.GetBytes(value, copy.AsSpan(this.binary.Length));
        this.Encrypt(copy).CopyTo(copy);
        this.binary = copy;
        this.Length = l;
#else
        var bl = this.byteLength;
        var chars = ArrayPool<char>.Shared.Rent(this.Length);
        this.byteLength = this.byteLength + Encodings.Utf8NoBom.GetByteCount(chars);
        var copy = new byte[this.byteLength];
        this.Decrypt().Slice(0, bl).CopyTo(copy);
        Encodings.Utf8NoBom.GetBytes(chars, 0, this.Length, copy, this.binary.Length);
        this.Encrypt(copy).CopyTo(copy);
        this.binary = copy;
        this.Length = l;
        ArrayPool<char>.Shared.Return(chars, true);
#endif
        return this;
    }

    public ProtectedString Append(string value)
    {
        return this.Append(value.AsSpan());
    }

    public ProtectedString Append(StringBuilder value)
    {
        return this.Append(value.AsSpan());
    }

    public ProtectedString Append(ProtectedString value)
    {
        return this.Append(value.UnprotectAsCharSpan());
    }

    public ProtectedString Prepend(ReadOnlySpan<char> value)
    {
        var l = this.Length + value.Length;
        if (this.value is not null)
        {
            var next = new char[l];
            this.Length = next.Length;
            var span = next.AsSpan();
            value.CopyTo(span);
            this.value.AsSpan().CopyTo(span.Slice(value.Length));
            this.value = new string(next);
            this.Length = this.value.Length;
        }

#if !NETLEGACY
        var bl = this.byteLength;
        this.byteLength = this.byteLength + Encodings.Utf8NoBom.GetByteCount(value);
        var copy = new byte[this.byteLength];
        this.Decrypt().Slice(0, bl).CopyTo(copy.AsSpan(value.Length));
        Encodings.Utf8NoBom.GetBytes(value, copy);
        this.Encrypt(copy).CopyTo(copy);
        this.binary = copy;
        this.Length = l;
#else
        var bl = this.byteLength;
        var chars = ArrayPool<char>.Shared.Rent(this.Length);
        this.byteLength = this.byteLength + Encodings.Utf8NoBom.GetByteCount(chars);
        var copy = new byte[this.byteLength];
        this.Decrypt().Slice(0, bl).CopyTo(copy.AsSpan(value.Length));
        Encodings.Utf8NoBom.GetBytes(chars, 0, this.Length, copy, 0);
        this.Encrypt(copy).CopyTo(copy);
        this.binary = copy;
        this.Length = l;
        ArrayPool<char>.Shared.Return(chars, true);
#endif
        return this;
    }

    /// <summary>
    /// Decrypts the inner data and returns a copy.
    /// </summary>
    /// <returns>Returns a copy of the data.</returns>
    public ReadOnlySpan<byte> Unprotect()
    {
        if (this.value is not null)
            return Encodings.Utf8NoBom.GetBytes(this.value);

        if (!this.IsProtected)
            return this.binary;

        return this.Decrypt();
    }

    public ReadOnlySpan<char> UnprotectAsCharSpan()
    {
        if (this.value is not null)
            return this.value.AsSpan();

        var bytes = this.Unprotect().Slice(0, this.byteLength);
#if !NETLEGACY
        var chars = new char[Encodings.Utf8NoBom.GetCharCount(bytes)];
        Encodings.Utf8NoBom.GetChars(bytes, chars);
        return chars;
#else
        var rental = ArrayPool<byte>.Shared.Rent(bytes.Length);
        var chars = Encodings.Utf8NoBom.GetChars(rental, 0, bytes.Length);
        ArrayPool<byte>.Shared.Return(rental, true);
        return chars;
#endif
    }

    public string UnprotectAsString()
    {
        if (this.value is not null)
            return this.value;

        var bytes = this.Unprotect().Slice(0, this.byteLength);

#if !NETLEGACY
        return Encodings.Utf8NoBom.GetString(bytes);
#else
        var rental = ArrayPool<byte>.Shared.Rent(bytes.Length);
        var str = Encodings.Utf8NoBom.GetString(rental, 0, bytes.Length);
        ArrayPool<byte>.Shared.Return(rental, true);
        return str;
#endif
    }

    public bool Equals(ProtectedString? other)
        => this.Equals(other, true);

    public bool Equals(ProtectedString? other, bool onlyWhenProtected)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (this.Length != other.Length)
            return false;

        if (!this.IsProtected && !other.IsProtected)
            return this.value?.Equals(other.value) == true;

        if (onlyWhenProtected && (!this.IsProtected || !other.IsProtected))
            return false;

        return this.Decrypt().SequenceEqual(other.Decrypt());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is ProtectedString str)
            return this.Equals(str);

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Id, this.IsProtected);
    }

    public override string ToString()
    {
        return "**********";
    }

    private ReadOnlySpan<byte> Encrypt(ReadOnlySpan<byte> bytes)
    {
        if (!this.IsProtected)
            return bytes;

        var action = ProtectionOptions.DataProtectionAction;
        return action(bytes, this.Id, DataProtectionActionType.Encrypt);
    }

    private ReadOnlySpan<byte> Decrypt()
    {
        if (!this.IsProtected)
            return this.binary;

        var action = ProtectionOptions.DataProtectionAction;
        return action(this.binary, this.Id, DataProtectionActionType.Decrypt);
    }
}