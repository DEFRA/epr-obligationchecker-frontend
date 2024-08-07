namespace FrontendObligationChecker.ViewModels.LargeProducer
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class LargeProducerFileViewModel
    {
        public string FileName { get; set; }

        public Stream FileContents { get; set; }
    }
}