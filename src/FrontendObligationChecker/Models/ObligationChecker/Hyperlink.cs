namespace FrontendObligationChecker.Models.ObligationChecker;

public class Hyperlink : ContentItem
{
    public string Url { get; }

    public string Description { get; }

    public string AnalyticsTrackingId { get; }

    public Hyperlink(string url, string description, string analyticsTrackingId, CompanySize companySize) : base(ContentType.Hyperlink, companySize)
    {
        Url = url;
        Description = description;
        AnalyticsTrackingId = analyticsTrackingId;
    }

    public Hyperlink(string url, string description, string analyticsTrackingId, CompanySize companySize, SellerType sellerType) : base(ContentType.Hyperlink, companySize, sellerType)
    {
        Url = url;
        Description = description;
        AnalyticsTrackingId = analyticsTrackingId;
    }
}