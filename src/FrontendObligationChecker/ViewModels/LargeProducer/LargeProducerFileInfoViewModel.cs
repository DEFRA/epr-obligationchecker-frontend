namespace FrontendObligationChecker.ViewModels.LargeProducer
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class LargeProducerFileInfoViewModel
    {
        public int ReportingYear { get; set; }

        public string DisplayFileSize { get; set; }

        public DateTime DateCreated { get; set; }
    }
}