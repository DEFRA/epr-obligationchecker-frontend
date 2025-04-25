namespace FrontendObligationChecker.Models.BlobReader
{
    using System.Diagnostics.CodeAnalysis;
    using Azure.Storage.Blobs.Models;

    [ExcludeFromCodeCoverage]
    public class PublicRegisterBlobModel
    {
        public string? Name { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime? LastModified { get; set; }

        public string? ContentLength { get; set; }

        public string? FileType { get; set; }

        public List<BlobItem> EnforcementActionItems { get; set; }
    }
}