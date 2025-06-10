namespace FrontendObligationChecker.ViewModels.PublicRegister
{
    public class GuidanceViewModel : BaseViewModel
    {
        public string PublishedDate { get; set; }

        public string LastUpdated { get; set; }

        public PublicRegisterFileViewModel ProducerRegisteredFile { get; set; }

        public PublicRegisterFileViewModel? ComplianceSchemeRegisteredFile { get; set; }

        public string DefraUrl { get; set; }

        public string DefraHelplineEmail { get; set; }

        public IEnumerable<EnforcementActionFileViewModel>? EnforcementActionFiles { get; set; }

        public EnforcementActionFileViewModel? EnglishEnforcementActionFile { get; set; }

        public EnforcementActionFileViewModel? WelshEnforcementActionFile { get; set; }

        public string ScottishEnforcementActionFileUrl { get; set; }

        public EnforcementActionFileViewModel? NortherIrishEnforcementActionFile { get; set; }
    }
}