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

    protected LargeProducerRegisterServiceException(SerializationInfo info, StreamingContext context)
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