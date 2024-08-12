﻿namespace FrontendObligationChecker.ViewModels;

using System.Diagnostics.CodeAnalysis;
using Models.ObligationChecker;

[ExcludeFromCodeCoverage]
public class AmountYouSupply
{
    public AmountYouSupply(Page page)
    {
        HasSingleUseDrinkContainers = CheckHasSingleUseDrinkContainers(page);

        var activityYesQuestions = GetNonSellerActivityYesQuestions(page);

        HasSingleActivity = activityYesQuestions.Count == 1;

        NonSellerYesActivities = activityYesQuestions
            .ConvertAll(x => string.IsNullOrWhiteSpace(x.AmountHandlePageText) ? x.AlternateTitle : x.AmountHandlePageText);

        IsParentCompany = CheckIsParentCompany(page);
    }

    public List<string> NonSellerYesActivities { get; }

    public bool HasSingleActivity { get; }

    public bool HasSingleUseDrinkContainers { get; }

    public bool IsParentCompany { get; }

    private static List<Question> GetNonSellerActivityYesQuestions(Page page)
    {
        return page.SessionPages
                    .Where(x => PagePath.IsActivityPagePath(x.Path))
                    .SelectMany(x => x.Questions)
                    .Where(x => x.Key != QuestionKey.SupplyingFilledPackaging && x.Answer == YesNo.Yes && !string.IsNullOrWhiteSpace(x.AlternateTitle))
                    .ToList();
    }

    private static bool CheckHasSingleUseDrinkContainers(Page page)
    {
        var placingDrinksOnMarket = page.FindPage(PagePath.PlaceDrinksOnMarket)?.Questions?.FirstOrDefault().Answer;
        var isPlacingDrinksOnMarket = placingDrinksOnMarket == YesNo.Yes;

        if (!isPlacingDrinksOnMarket)
        {
            return false;
        }

        var drinkContainersMaterials = page.FindPage(PagePath.MaterialsForDrinksContainers)?.Questions?.FirstOrDefault().Answer;
        bool isUsingDrinkContainersMaterials = drinkContainersMaterials != "0";

        if (!isUsingDrinkContainersMaterials)
        {
            return false;
        }

        var answer = page.FindPage(PagePath.ContainerVolume)?.Questions?.FirstOrDefault().Answer;
        return answer == YesNo.Yes;
    }

    private static bool CheckIsParentCompany(Page page)
    {
        var typeOfOrganisationPage = page.FindPage(PagePath.TypeOfOrganisation);

        return typeOfOrganisationPage.Questions
            .Exists(x => x.Answer == "parent");
    }
}