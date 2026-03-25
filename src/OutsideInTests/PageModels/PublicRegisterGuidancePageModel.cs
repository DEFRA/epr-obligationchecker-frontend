namespace OutsideInTests.PageModels;

public class PublicRegisterGuidancePageModel : PageModelBase, IPageModelFactory<PublicRegisterGuidancePageModel>
{
    public string? Heading => _document.QuerySelector("#public-register-heading")?.TextContent.Trim();

    public string? Description => _document.QuerySelector("#public-register-description")?.TextContent.Trim();

    public IReadOnlyList<string> DownloadLinks =>
        _document.QuerySelectorAll("a.gem-c-attachment__link")
            .Select(a => a.TextContent.Trim())
            .ToList();

    public IReadOnlyList<string> DownloadHrefs =>
        _document.QuerySelectorAll("a.gem-c-attachment__link")
            .Select(a => a.GetAttribute("href") ?? "")
            .ToList();

    private PublicRegisterGuidancePageModel(string html) : base(html) { }

    public static PublicRegisterGuidancePageModel FromContent(string html) => new(html);
}
