using Newtonsoft.Json;

namespace FrontendObligationChecker.Models.ObligationChecker;

public class ContentItem
{
    [JsonConstructor]
    public ContentItem()
    {
    }

    public ContentItem(ContentType contentType, CompanySize companySize = CompanySize.All, SellerType sellerType = SellerType.All) : this(contentType, null, null, companySize, sellerType, AssociationType.All)
    {
        ContentType = contentType;
    }

    public ContentItem(
        ContentType contentType,
        List<string> contentItems,
        CompanySize companySize,
        SellerType sellerType = SellerType.All,
        bool? requiresNationData = null) : this(contentType, null, contentItems, companySize, sellerType, AssociationType.All, requiresNationData)
    {
    }

    public ContentItem(
        ContentType contentType,
        string content,
        CompanySize companySize,
        SellerType sellerType = SellerType.All,
        bool? requiresNationData = null,
        AssociationType associationType = AssociationType.All) : this(contentType, content, null, companySize, sellerType, associationType, requiresNationData)
    {
    }

    public ContentItem(
        ContentType contentType,
        string content,
        SellerType sellerType = SellerType.All,
        AssociationType associationType = AssociationType.All) : this(contentType, content, null, null, sellerType, associationType)
    {
    }

    private ContentItem(
        ContentType contentType,
        string? content,
        List<string>? contentItems,
        CompanySize? companySize,
        SellerType sellerType,
        AssociationType? associationType,
        bool? requiresNationData = null)
    {
        ContentType = contentType;
        Content = content;
        ContentItems = contentItems;
        SellerType = sellerType;

        if (companySize != null)
        {
            CompanySize = companySize.Value;
        }

        if (associationType != null)
        {
            AssociationType = associationType.Value;
        }

        RequiresNationData = requiresNationData;
    }

    public AssociationType AssociationType { get; set; }

    public ContentType ContentType { get; set; }

    public CompanySize CompanySize { get; set; }

    public SellerType SellerType { get; set; }

    public bool? RequiresNationData { get; set; }

    public string Content { get; set; }

    public List<string> ContentItems { get; set; }
}