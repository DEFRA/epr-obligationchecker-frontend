using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.Services.PageService.Interfaces;

namespace FrontendObligationChecker.UnitTests.Helpers;

public static class PageServiceExtension
{
    public static async Task SetVisitedPages(this PageService pageService, VisitedPages visitedPages)
    {
        string pagePath = PagePath.TypeOfOrganisation;

        while (!string.IsNullOrEmpty(pagePath))
        {
            switch (pagePath)
            {
                case PagePath.TypeOfOrganisation:
                    pagePath = await GetNextOfTypeOfOrganisation(pageService, visitedPages.TypeOfOrganisation);
                    continue;

                case PagePath.AnnualTurnover:
                    pagePath = await GetNextOfAnnualTurnover(pageService, visitedPages.AnnualTurnover);
                    continue;

                case PagePath.MaterialsForDrinksContainers:
                    pagePath = await GetNextOfMaterialsForDrinksContainers(pageService, visitedPages.MaterialsForDrinksContainers);
                    continue;

                case PagePath.AmountYouSupply:
                    pagePath = await GetNextOfAmountYouSupply(pageService, visitedPages.AmountYouSupply);
                    continue;

                case PagePath.PlaceDrinksOnMarket:
                case PagePath.ContainerVolume:
                case PagePath.OwnBrand:
                case PagePath.UnbrandedPackaging:
                case PagePath.ImportingProducts:
                case PagePath.SupplyingEmptyPackaging:
                case PagePath.HiringLoaning:
                case PagePath.OnlineMarketplace:
                case PagePath.SupplyingFilledPackaging:
                    {
                        pagePath = await GetNextOfYesNoPage(pageService, pagePath, visitedPages);
                        continue;
                    }

                case PagePath.NoActionNeeded:
                case PagePath.WhatYouNeedToDo:
                    return;

                default:
                    throw new ArgumentException("Unhandled case");
            }
        }
    }

    private static async Task<string> GetNextOfTypeOfOrganisation(IPageService pageService, TypeOfOrganisation typeOfOrganisation)
    {
        if (typeOfOrganisation == TypeOfOrganisation.NotSet)
        {
            return string.Empty;
        }

        Page page = await pageService.SetAnswersAndGetPageAsync(PagePath.TypeOfOrganisation, new PageForm(typeOfOrganisation).FormCollection);

        return PageFinder.GetNextPath(page);
    }

    private static async Task<string> GetNextOfAnnualTurnover(IPageService pageService, AnnualTurnover annualTurnover)
    {
        if (annualTurnover == AnnualTurnover.NotSet)
        {
            return string.Empty;
        }

        Page page = await pageService.SetAnswersAndGetPageAsync(PagePath.AnnualTurnover, new PageForm(annualTurnover).FormCollection);

        return PageFinder.GetNextPath(page);
    }

    private static async Task<string> GetNextOfMaterialsForDrinksContainers(IPageService pageService, MaterialsForDrinksContainers materialsForDrinksContainers)
    {
        if (materialsForDrinksContainers == MaterialsForDrinksContainers.NotSet)
        {
            return string.Empty;
        }

        Page page = await pageService.SetAnswersAndGetPageAsync(PagePath.MaterialsForDrinksContainers, new PageForm(materialsForDrinksContainers).FormCollection);

        return PageFinder.GetNextPath(page);
    }

    private static async Task<string> GetNextOfAmountYouSupply(IPageService pageService, AmountYouSupply amountYouHandle)
    {
        if (amountYouHandle == AmountYouSupply.NotSet)
        {
            return string.Empty;
        }

        Page page = await pageService.SetAnswersAndGetPageAsync(PagePath.AmountYouSupply, new PageForm(amountYouHandle).FormCollection);

        return PageFinder.GetNextPath(page);
    }

    private static async Task<string> GetNextOfYesNoPage(IPageService pageService, string pagePath, VisitedPages visitedPages)
    {
        YesNo yesNo = pagePath switch
        {
            PagePath.PlaceDrinksOnMarket => visitedPages.PlaceDrinksOnMarket,
            PagePath.ContainerVolume => visitedPages.ContainerVolume,
            PagePath.OwnBrand => visitedPages.OwnBrand,
            PagePath.UnbrandedPackaging => visitedPages.UnbrandedPackaging,
            PagePath.ImportingProducts => visitedPages.ImportingProducts,
            PagePath.SupplyingEmptyPackaging => visitedPages.SellingEmptyPackaging,
            PagePath.HiringLoaning => visitedPages.HiringLoaning,
            PagePath.OnlineMarketplace => visitedPages.OnlineMarketplace,
            PagePath.SupplyingFilledPackaging => visitedPages.SupplyingFilledPackaging,
            _ => YesNo.NotSet
        };

        if (yesNo == YesNo.NotSet)
        {
            return string.Empty;
        }

        string questionKey = QuestionKey.GetYesNoPageQuestionKey(pagePath);

        Page page = await pageService.SetAnswersAndGetPageAsync(pagePath, new PageForm(questionKey, yesNo).FormCollection);

        return PageFinder.GetNextPath(page);
    }

}