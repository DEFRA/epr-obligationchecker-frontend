namespace FrontendObligationChecker.Exceptions;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[Serializable]
[ExcludeFromCodeCoverage]
public class LargeProducerRegisterServiceException : Exception
{
    public LargeProducerRegisterServiceException()
    {
    }

    public LargeProducerRegisterServiceException(string message) : base(message)
    {
    }

    public LargeProducerRegisterServiceException(string message, Exception inner)
        : base(message, inner)
    {
    }

#pragma warning disable SYSLIB0051
    protected LargeProducerRegisterServiceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
#pragma warning restore SYSLIB0051

}