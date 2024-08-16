namespace FrontendObligationChecker.Models.BlobReader
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class BlobModel
    {
        public string Name { get; set; }

        public long? ContentLength { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}