namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;
using FrontendObligationChecker.ViewModels.LargeProducer;

[ExcludeFromCodeCoverage]
public class LargeProducerRegisterViewModel : BaseViewModel
{
    public IEnumerable<LargeProducerFileInfoViewModel> LatestAllNationsFiles { get; set; }
}