namespace FrontendObligationChecker.Models.BlobReader
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.AspNetCore.Routing.Constraints;

    [ExcludeFromCodeCoverage]
    public class PublicRegisterFaileModel
    {
        public string FileName { get; set; }
        public Stream FileContent { get; set; }
    }
}