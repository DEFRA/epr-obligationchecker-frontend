namespace FrontendObligationChecker.Models.BlobReader
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Routing.Constraints;

    [ExcludeFromCodeCoverage]
    public class PublicRegisterFileModel
    {
        public string FileName { get; set; }

        public Stream FileContent { get; set; }
    }
}