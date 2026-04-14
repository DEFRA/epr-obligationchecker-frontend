namespace OutsideInTests.PageModels;

public class LargeProducerRegisterPageModel : PageModelBase, IPageModelFactory<LargeProducerRegisterPageModel>
{
    public string? Heading => _document.QuerySelector("h1.govuk-heading-l")?.TextContent.Trim();

    /// <summary>
    /// Links in the "List of large producers" section (pre-2025 reports).
    /// These are plain anchor tags whose href points to /large-producers/report.
    /// </summary>
    public IReadOnlyList<FileEntry> Files =>
        _document.QuerySelectorAll("a.govuk-link")
            .Where(a => (a.GetAttribute("href") ?? "").Contains("/large-producers/report"))
            .Select(a => new FileEntry(
                a.GetAttribute("href") ?? "",
                a.TextContent.Trim()))
            .ToList();

    /// <summary>
    /// Links in the "Register of producers" section (archive page).
    /// These are plain anchor tags whose href points to /public-register/report.
    /// </summary>
    public IReadOnlyList<RegisterOfProducersEntry> RegisterOfProducersLinks =>
        _document.QuerySelectorAll("a.govuk-link")
            .Where(a => (a.GetAttribute("href") ?? "").Contains("/public-register/report"))
            .Select(a => new RegisterOfProducersEntry(
                a.GetAttribute("href") ?? "",
                a.TextContent.Trim()))
            .ToList();

    private LargeProducerRegisterPageModel(string html) : base(html) { }

    public static LargeProducerRegisterPageModel FromContent(string html) => new(html);

    public record FileEntry(string Href, string LinkText);

    public record RegisterOfProducersEntry(string Href, string LinkText);
}
