namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class LargeProducerRegisterViewModel : BaseViewModel
{
    public Dictionary<string, string> HomeNationFileSizeMapping { get; set; }
}