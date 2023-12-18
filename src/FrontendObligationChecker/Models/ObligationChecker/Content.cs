namespace FrontendObligationChecker.Models.ObligationChecker;
public class Content
{
    public List<ContentItem> ContentItems { get; set; }

    public List<ContentItem> GetRelatedContentItems(AssociationType? associationType) =>
        ContentItems.Where(x => associationType == null || x.AssociationType.HasFlag(associationType)).ToList();

    public List<ContentItem> GetObligatedContentItems(CompanyModel companyModel, AssociationType associationType)
    {
        return ContentItems.Where(contentItem =>
            contentItem.CompanySize.HasFlag(companyModel.CompanySize) &&
            contentItem.SellerType.HasFlag(companyModel.SellerType) &&
            (contentItem.AssociationType == AssociationType.NotSet || contentItem.AssociationType.HasFlag(associationType)) &&
            (contentItem.RequiresNationData is null || contentItem.RequiresNationData == companyModel.RequiresNationData)).ToList();
    }

}