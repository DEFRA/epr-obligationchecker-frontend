namespace FrontendObligationChecker.Exceptions;
using System.Diagnostics.CodeAnalysis;

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
}