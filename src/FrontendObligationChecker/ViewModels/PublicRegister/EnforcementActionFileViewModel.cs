namespace FrontendObligationChecker.ViewModels.PublicRegister
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class EnforcementActionFileViewModel
    {
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public int ContentFileLength { get; set; }

        public Stream FileContents { get; set; }
    }
}