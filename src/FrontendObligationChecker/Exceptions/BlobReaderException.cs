namespace FrontendObligationChecker.Exceptions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[Serializable]
[ExcludeFromCodeCoverage]
public class BlobReaderException : Exception
{
    public BlobReaderException()
    {
    }

    public BlobReaderException(string message) : base(message)
    {
    }

    public BlobReaderException(string message, Exception inner)
        : base(message, inner)
    {
    }

#pragma warning disable SYSLIB0051
    protected BlobReaderException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
#pragma warning restore SYSLIB0051
}