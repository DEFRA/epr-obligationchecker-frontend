namespace FrontendObligationChecker.ViewModels.PublicRegister
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class GuidanceViewModel : BaseViewModel
    {
        public string PublishedDate { get; set; }

        public string LastUpdated { get; set; }

        public PublicRegisterFileViewModel ProducerRegisteredFile { get; set; }

        public PublicRegisterFileViewModel? ComplianceSchemeRegisteredFile { get; set; }

        public string DefraUrl { get; set; }

        public IEnumerable<EnforcementActionFileViewModel>? EnforcementActionFiles { get; set; }
    }
}