using System;

namespace Bearz.Security.Cryptography;

[CLSCompliant(false)]
public interface IUnsignedCsrng
{
    ushort NextUInt16();

    uint NextUInt32();

    long NextUInt64();
}