namespace FrontendObligationChecker.ViewModels.PublicRegister
{
    public class GuidanceViewModel : BaseViewModel
    {
        public string PublishedDate { get; set; }

        public string LastUpdated { get; set; }

        public PublicRegisterFileViewModel ProducerRegisteredFile { get; set; } = new PublicRegisterFileViewModel();

        public PublicRegisterFileViewModel ComplianceSchemeRegisteredFile { get; set; } = new PublicRegisterFileViewModel();

        public string DefraUrl { get; set; }

        public string DefraHelplineEmail { get; set; }

        public IEnumerable<EnforcementActionFileViewModel> EnforcementActionFiles { get; set; } =[];

        public EnforcementActionFileViewModel? EnglishEnforcementActionFile { get; set; }

        public EnforcementActionFileViewModel? WelshEnforcementActionFile { get; set; }

        public string ScottishEnforcementActionFileUrl { get; set; }

        public EnforcementActionFileViewModel? NortherIrishEnforcementActionFile { get; set; }
    }
}