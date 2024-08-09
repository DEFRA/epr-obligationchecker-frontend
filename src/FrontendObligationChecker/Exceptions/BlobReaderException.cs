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

    protected BlobReaderException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        base.GetObjectData(info, context);
    }
}