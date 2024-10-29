using FrontendObligationChecker.Generators;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;
using FrontendObligationChecker.Services.PageService.Interfaces;
using FrontendObligationChecker.Services.Session.Interfaces;
using Microsoft.Extensions.Options;

using YesNo = FrontendObligationChecker.Models.ObligationChecker.YesNo;

namespace FrontendObligationChecker.Services.PageService;
public class PageService : IPageService
{
    private readonly IJourneySession _journeySession;
    private readonly ExternalUrlsOptions _externalUrls;
    private IEnumerable<Page>? _pages;

    public PageService(IJourneySession journeySession, IOptions<ExternalUrlsOptions> externalUrls)
    {
        _journeySession = journeySession;
        _externalUrls = externalUrls.Value;
    }

    public async Task<Page?> GetPageAsync(string path)
    {
        var sessionJourney = await _journeySession.GetAsync();

        if (!GlobalData.ByPassSessionValidation && sessionJourney == null && path != PagePath.TypeOfOrganisation)
        {
            return null;
        }

        await InitialiseAsync();

        var page = _pages.SingleOrDefault(x => x.Path == path);
        if (page == null)
        {
            return null;
        }

        await SetAnswersAsync(page);

        await SetPreviousPageAsync(page);

        await SetAssociationTypeAsync(page);

        await SetAllPagesAnswersAsync();

        await SetAlternateDescriptionsAsync(page);

        await SetCompanyModelAsync(page);

        page.SessionPages = _pages.Where(x => x.Path != PagePath.NoActionNeeded && x.Path != PagePath.WhatYouNeedToDo)
            .ToList();

        return page;
    }

    public async Task<Page?> SetAnswersAndGetPageAsync(string path, IFormCollection formCollection)
    {
        var sessionJourney = await _journeySession.GetAsync();

        if (!GlobalData.ByPassSessionValidation && sessionJourney == null && path != PagePath.TypeOfOrganisation)
        {
            return null;
        }

        await InitialiseAsync();

        var page = await GetPageAsync(path);
        if (page == null)
        {
            return null;
        }

        if (sessionJourney != null && HasAnswersChanged(page, sessionJourney, formCollection))
        {
            await _journeySession.RemovePagesAfterCurrentAsync(page);
        }

        page.Questions.ForEach(question => question.SetAnswer(formCollection[question.Key]));

        if (!page.HasError)
        {
            await _journeySession.AddPageAsync(page);
        }

        return page;
    }

    private static bool HasAnswersChanged(Page page, SessionJourney? sessionJourney, IFormCollection formCollection)
    {
        var providedAnswers = page.Questions.ToDictionary(q => q.Key, q => string.Join(",", formCollection[q.Key].ToArray()));
        var storedAnswers = sessionJourney.Pages.SingleOrDefault(x => x.Path == page.Path)?.Questions
            .ToDictionary(q => q.Key, q => q.Answer);

        return storedAnswers != null && providedAnswers.Except(storedAnswers).Any();
    }

    private async Task InitialiseAsync()
    {
        string eprGuidanceUrl = string.Empty;

        if (!string.IsNullOrEmpty(_externalUrls?.EprGuidance))
        {
            eprGuidanceUrl = new Uri(_externalUrls.EprGuidance).OriginalString;
        }

        _pages ??= PageGenerator.Create(eprGuidanceUrl);
    }

    private async Task SetAnswersAsync(Page page)
    {
        var sessionJourney = await _journeySession.GetAsync();
        var sessionPage = sessionJourney?.Pages.SingleOrDefault(x => x.Path == page.Path);

        if (sessionPage == null) return;

        page.Questions.ForEach(question =>
        {
            var sessionQuestion = sessionPage.Questions.SingleOrDefault(x => x.Key == question.Key);
            if (sessionQuestion != null)
            {
                question.SetAnswer(sessionQuestion.Answer);
            }
        });
    }

    private async Task SetPreviousPageAsync(Page page)
    {
        var sessionJourney = await _journeySession.GetAsync();
        var sessionPaths = sessionJourney?.Pages.Select(x => x.Path).ToList();
        if (sessionPaths != null)
        {
            page.PreviousPage = _pages.Where(x => x.Index < page.Index && sessionPaths.Contains(x.Path))
                .MaxBy(x => x.Index);
            if (page.PreviousPage != null)
            {
                await SetAnswersAsync(page.PreviousPage);
            }
        }
    }

    private async Task SetAllPagesAnswersAsync()
    {
        var sessionJourney = await _journeySession.GetAsync();
        var sessionPages = sessionJourney?.Pages;

        if (sessionPages == null) return;

        foreach (var sessionPage in sessionPages)
        {
            var page = _pages.FirstOrDefault(x => x.Path == sessionPage.Path);
            await SetAnswersAsync(page);
        }
    }

    private async Task SetAssociationTypeAsync(Page page)
    {
        var sessionJourney = await _journeySession.GetAsync();

        var sessionPage = sessionJourney?.Pages.Find(x => x.Path == PagePath.TypeOfOrganisation);
        if (sessionPage != null)
        {
            var answer = sessionPage.Questions.Find(q => q.Key == QuestionKey.TypeOfOrganisation).Answer;
            page.AssociationType = answer switch
            {
                "parent" => AssociationType.Parent,
                "subsidiary" => AssociationType.Subsidiary,
                "individual" => AssociationType.Individual,
                _ => page.AssociationType
            };
        }
    }

    private async Task SetAlternateDescriptionsAsync(Page page)
    {
        if (page.Path != PagePath.AmountYouSupply && page.Path != PagePath.AnnualTurnover) return;

        if (page.Path == PagePath.AnnualTurnover)
        {
            SetDescriptionForAnnualTurnover(page);
        }

        if (page.Path == PagePath.AmountYouSupply)
        {
            var handlePackagingCount = _pages
                                            .Where(x => PagePath.IsActivityPagePath(x.Path))
                                            .SelectMany(x => x.Questions)
                                            .Count(x => x.SelectedOption is { Next: OptionPath.Primary });

            if (handlePackagingCount > 0)
            {
                page.FirstQuestion.Title = handlePackagingCount > 1
                    ? "SingleQuestion.AmountYouSupply.QuestionTitle"
                    : "SingleQuestion.AmountYouSupply.QuestionTitleSingular";

                if (page.AssociationType == AssociationType.Parent)
                {
                    page.FirstQuestion.AlternateDescription = handlePackagingCount > 1
                        ? "AmountYouSupply.Description"
                        : "AmountYouSupply.DescriptionAlternate";
                }
            }
        }
    }

    private async Task SetDescriptionForAnnualTurnover(Page page)
    {
        var sessionJourney = await _journeySession.GetAsync();
        var sessionPage = sessionJourney?.Pages.Find(x => x.Path == PagePath.TypeOfOrganisation);

        if (sessionPage != null)
        {
            var answer = sessionPage.Questions.Find(q => q.Key == QuestionKey.TypeOfOrganisation).Answer;

            page.AdditionalDescription = answer switch
            {
                "parent" => page.AdditionalDescription,
                "subsidary" => null,
                "individual" => null,
                _ => page.AdditionalDescription
            };
        }
    }

    private async Task SetCompanyModelAsync(Page page)
    {
        if (page.Path != PagePath.WhatYouNeedToDo)
        {
            return;
        }

        var amountOfPackaging = _pages
            .SingleOrDefault(x => x.Path == PagePath.AmountYouSupply)?
            .FirstQuestion
            .SelectedOption;

        page.CompanyModel = new CompanyModel();

        // Decide company size
        SetCompanySize(page.CompanyModel, amountOfPackaging);

        // Decide if company requires nation data reporting
        SetRequiresNationData(page.CompanyModel, amountOfPackaging);

        // Decide if company is seller only
        SetSellerType(page.CompanyModel);
    }

    private void SetCompanySize(CompanyModel companyModel, Option? amountOfPackaging)
    {
        const string amountOfPackagingLarge = "3";
        const string turnoverLarge = "3";

        var turnover = _pages
            .SingleOrDefault(x => x.Path == PagePath.AnnualTurnover)?
            .FirstQuestion
            .SelectedOption;

        if (turnoverLarge.Equals(turnover?.Value) && amountOfPackagingLarge.Equals(amountOfPackaging?.Value))
        {
            companyModel.CompanySize = CompanySize.Large;
        }
        else
        {
            companyModel.CompanySize = CompanySize.Small;
        }
    }

    private void SetSellerType(CompanyModel companyModel)
    {
        var activityPages = _pages.Where(x => PagePath.IsActivityPagePath(x.Path)).ToList();

        if (activityPages.Count == 0)
        {
            return;
        }

        var sellerQuestion = activityPages
            .SelectMany(x => x.Questions)
            .First(x => x.Key == QuestionKey.SellingEmptyPackaging);

        var supplyFilledPackagingQuestion = activityPages
            .SelectMany(x => x.Questions)
            .First(x => x.Key == QuestionKey.SupplyingFilledPackaging);

        // Pages belonging to producer activities other than Seller
        string[] producerActivityPagePaths =
        {
            PagePath.OwnBrand,
            PagePath.UnbrandedPackaging,
            PagePath.ImportingProducts,
            PagePath.HiringLoaning,
            PagePath.OnlineMarketplace
        };

        var producerActivities = _pages
            .Where(x => producerActivityPagePaths.Contains(x.Path))
            .Select(x => x.FirstQuestion.SelectedOption).ToList();

        if (producerActivities.All(x => x!= null && x.Value == YesNo.No) && sellerQuestion.Answer == YesNo.Yes && supplyFilledPackagingQuestion.Answer == YesNo.Yes)
        {
            companyModel.SellerType = SellerType.SellerOnly;
        }
        else
        {
            companyModel.SellerType = SellerType.NotSellerOnly;
        }
    }

    private void SetRequiresNationData(CompanyModel companyModel, Option? amountOfPackaging)
    {
        const string amountOfPackagingNonObligated = "1";

        // Pages belonging to producer activities that are not Brand Owner or Packer/Filler
        string[] producerActivityPagePaths =
        {
            PagePath.ImportingProducts,
            PagePath.SupplyingEmptyPackaging,
            PagePath.HiringLoaning,
            PagePath.OnlineMarketplace,
            PagePath.SupplyingFilledPackaging
        };

        var selectedOptions = _pages
            .Where(x => producerActivityPagePaths.Contains(x.Path))
            .Select(x => x.FirstQuestion.SelectedOption).ToList();

        companyModel.RequiresNationData =
            amountOfPackaging?.Value != amountOfPackagingNonObligated
            && selectedOptions.Exists(x => x != null && x.Value == YesNo.Yes);
    }
}