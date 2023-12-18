namespace FrontendObligationChecker.Models.ObligationChecker;

public class CompanyModel
{
    public CompanySize CompanySize { get; set; }

    public bool RequiresNationData { get; set; }

    public SellerType SellerType { get; set; }
}