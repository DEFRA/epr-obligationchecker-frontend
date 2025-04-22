namespace FrontendObligationChecker.ViewModels.PublicRegister
{
    public class GuidanceViewModel : BaseViewModel
    {
        public string PublishedDate { get; set; }

        public string LastUpdated { get; set; }

        public PublicRegisterFileViewModel ProducerRegisteredFile { get; set; }

        public PublicRegisterFileViewModel ComplianceSchemeRegisteredFile { get; set; }
    }
}