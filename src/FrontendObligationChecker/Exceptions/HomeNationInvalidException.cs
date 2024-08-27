namespace FrontendObligationChecker.Exceptions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[Serializable]
[ExcludeFromCodeCoverage]
public sealed class HomeNationInvalidException : Exception
{
    public HomeNationInvalidException()
    {
    }

    public HomeNationInvalidException(string message) : base(message)
    {
    }

#pragma warning disable SYSLIB0051
    private HomeNationInvalidException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
#pragma warning restore SYSLIB0051

}