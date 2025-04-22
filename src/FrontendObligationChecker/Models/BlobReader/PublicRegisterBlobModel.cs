namespace FrontendObligationChecker.Models.BlobReader
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PublicRegisterBlobModel
    {
        public string? Name { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime? LastModified { get; set; }

        public string? ContentLength { get; set; }

        public string? FileType { get; set; }

        public Stream FileContents { get; set; }
    }
}