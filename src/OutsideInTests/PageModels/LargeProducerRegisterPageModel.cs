namespace OutsideInTests.PageModels;

public class LargeProducerRegisterPageModel : PageModelBase, IPageModelFactory<LargeProducerRegisterPageModel>
{
    public string? Heading => _document.QuerySelector("h1.govuk-heading-l")?.TextContent.Trim();

    public IReadOnlyList<FileEntry> Files =>
        _document.QuerySelectorAll("ul.govuk-list > li")
            .Where(li => li.QuerySelector("h3.govuk-heading-s") != null)
            .Select(li => new FileEntry(
                li.QuerySelector("h3.govuk-heading-s")?.TextContent.Trim() ?? "",
                li.QuerySelector("a.govuk-link")?.GetAttribute("href") ?? "",
                li.QuerySelector("a.govuk-link")?.TextContent.Trim() ?? ""))
            .ToList();

    private LargeProducerRegisterPageModel(string html) : base(html) { }

    public static LargeProducerRegisterPageModel FromContent(string html) => new(html);

    public record FileEntry(string Year, string Href, string LinkText);
}
