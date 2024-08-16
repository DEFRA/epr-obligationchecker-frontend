namespace FrontendObligationChecker.ViewModels.LargeProducer
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class LargeProducerFileInfoViewModel
    {
        public string DisplayFileSize { get; set; }

        public DateTime DateCreated { get; set; }
    }
}