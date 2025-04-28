namespace FrontendObligationChecker.Exceptions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [Serializable]
    [ExcludeFromCodeCoverage]
    public class PublicRegisterServiceException : Exception
    {
        public PublicRegisterServiceException()
        {
        }

        public PublicRegisterServiceException(string message) : base(message)
        {
        }

        public PublicRegisterServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }

#pragma warning disable SYSLIB0051
        protected PublicRegisterServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}