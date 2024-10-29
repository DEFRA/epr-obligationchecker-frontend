using FrontendObligationChecker.Services.NextFinder;
using FrontendObligationChecker.Services.NextFinder.Interfaces;

using Newtonsoft.Json;

namespace FrontendObligationChecker.Models.ObligationChecker;

public class Page
{
    private string _additionalDescription;

    public INextFinder NextFinder { get; init; } = new OptionFinder();

    public short Index { get; init; }

    public string Path { get; init; } = default!;

    public string AlternateRowTitle { get; init; } = default!;

    public Dictionary<OptionPath, string> Titles { get; init; } = new();

    public string TitleCaption { get; init; } = default;

    public bool HasTitleCaption => !string.IsNullOrEmpty(TitleCaption);

    public Dictionary<string, Content>? Contents { get; init; } = new();

    public PageType PageType { get; init; } = PageType.SingleQuestion;

    public PageGroup PageGroup { get; set; } = PageGroup.None;

    public string? BackLinkPath => PreviousPage?.Path;

    public List<Question> Questions { get; init; } = new();

    public Dictionary<OptionPath, string> Paths { get; init; } = new();

    public Page? PreviousPage { get; set; }

    public AssociationType AssociationType { get; set; }

    public CompanyModel CompanyModel { get; set; }

    public Dictionary<OptionPath, string> AdditionalDescriptions { get; init; } = new();

    public string GetAlternateTitleFirstParagraph()
    {
        return IsPackagingActivitiesPage() ? AlternateTitleFirstParagraph : null;
    }

    public List<string> AlternateTitleSubContents { get; init; } = new();

    public string AlternateTitleFirstParagraph { get; init; }

    public string AlternateTitleAlternativeFirstParagraph { get; init; }

    public string View => PageType.ToString();

    public bool HasError => Questions.Exists(question => question.HasError);

    public bool HasBackLink => !string.IsNullOrEmpty(BackLinkPath);

    public bool IsBackButtonHidden { get; init; }

    public string? AdditionalDescription
    {
        get
        {
            return PreviousPage switch
            {
                null => null,
                _ => AdditionalDescriptions.FirstOrDefault(x => x.Key == PreviousPage.Next().Key).Value
            };
        }

        // Changed from 'init' to 'set' due to potetially being changed at runtime. AdditionalDescription
        // value could changed depending on answers selected from a previous page.
        set
        {
            _additionalDescription = value;
        }
    }

    public Page? FindPage(string path)
    {
        return SessionPages.Find(x => x.Path == path);
    }

    [JsonIgnore]
    public Question? FirstQuestion => Questions.FirstOrDefault();

    public Dictionary<string, string> Errors => Questions
        .Where(question => question.HasError)
        .Select(question => new
        {
            question.Key,
            question.ErrorMessage
        })
        .ToDictionary(d => d.Key, d => d.ErrorMessage);

    public List<Page> SessionPages { get; set; }

    public string NextValue() => Next().Value;

    private KeyValuePair<OptionPath, string> Next()
    {
        var questionPath = NextFinder.Next(Questions);
        return Paths.FirstOrDefault(x => x.Key == questionPath);
    }

    public string Title =>
        PreviousPage switch
        {
            null => Titles.FirstOrDefault().Value,
            _ => Titles.FirstOrDefault(x => x.Key == PreviousPage.Next().Key).Value
        };

    private Content Content =>
        PreviousPage switch
        {
            null => Contents.FirstOrDefault().Value,
            _ => Contents.FirstOrDefault(x => x.Key == PreviousPage.Path).Value
        };

    public bool IsPageHeading { get; init; }

    public List<ContentItem> GetRelatedContentItems() => Content.GetRelatedContentItems(AssociationType);

    public List<ContentItem> GetObligatedContentItems(AssociationType associationType) => Contents.FirstOrDefault(x
        => x.Key == PagePath.WhatYouNeedToDo).Value.GetObligatedContentItems(CompanyModel, associationType);

    public IEnumerable<string> InsetText { get; init; } = Enumerable.Empty<string>();

    public List<string> GetAlternateTitles()
    {
        switch (Path)
        {
            case PagePath.PlaceDrinksOnMarket:
            case PagePath.MaterialsForDrinksContainers:
                return new List<string>();

            default:
                if (IsPackagingActivitiesPage())
                    return new List<string>();

                return PreviousPage switch
                {
                    null => new List<string>(),
                    _ => PreviousPage.Questions
                        .Where(x => x.Answer == YesNo.Yes && !string.IsNullOrWhiteSpace(x.AlternateTitle))
                        .Select(x => x.AlternateTitle).ToList()
                };
        }
    }

    public List<string> GetAlternateTitlesSubContent()
    {
        if (!IsPackagingActivitiesPage())
        {
            return new List<string>();
        }

        return AlternateTitleSubContents;
    }

    private bool IsPackagingActivitiesPage() =>
        Path is PagePath.OwnBrand or PagePath.UnbrandedPackaging or PagePath.ImportingProducts or
                PagePath.SupplyingEmptyPackaging or PagePath.HiringLoaning or PagePath.OnlineMarketplace or PagePath.SupplyingFilledPackaging;
}