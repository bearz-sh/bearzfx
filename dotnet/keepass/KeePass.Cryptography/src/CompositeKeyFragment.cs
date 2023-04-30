using Bearz.Security.Cryptography;

namespace Bearz.KeePass.Cryptography;

public class CompositeKeyFragment : ICompositeKeyFragment
{
    private ProtectedBuffer binary = ProtectedBuffer.Empty; 

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ReadOnlySpan<byte> ToReadOnlySpan()
        => this.binary.Unprotect();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        this.binary.Clear();
    }

    protected void SetData(byte[] data)
    {
        this.binary = new ProtectedBuffer(data, true);
    }
}