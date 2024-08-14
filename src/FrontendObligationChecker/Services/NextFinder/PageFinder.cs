using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.Services.NextFinder;

public static class PageFinder
{
    public static string GetNextPath(Page page)
    {
        switch (page.Path)
        {
            case PagePath.StartPage:
                return GetNextOfStartPage(page);
            case PagePath.SupplyingFilledPackaging:
                return GetNextOfHandlePackaging(page);
            case PagePath.AmountYouSupply:
                return GetNextOfAmountOfPackaging(page);
            case PagePath.MaterialsForDrinksContainers:
                return GetNextOfMaterialsForDrinksContainers(page);
            case PagePath.PlaceDrinksOnMarket:
                return GetNextOfSingleUseContainersDrs(page);
            case PagePath.ContainerVolume:
                return PagePath.AmountYouSupply;
            default:
                return page.NextValue();
        }
    }

    private static string GetNextOfStartPage(Page page)
    {
        return PagePath.TypeOfOrganisation;
    }

    private static string GetNextOfMaterialsForDrinksContainers(Page page)
    {
        var answers = GetAnswers(page);

        if (answers[QuestionKey.MaterialsForDrinksContainers] == YesNoAnswer.Yes)
        {
            return PagePath.ContainerVolume;
        }

        return PagePath.AmountYouSupply;
    }

    private static string GetNextOfHandlePackaging(Page page)
    {
        var answers = GetActivityAnswers(page);

        if (IsWasteCost(answers) || IsNationData(answers) || IsEndConsumerPackagingSeller(answers))
        {
            return PagePath.PlaceDrinksOnMarket;
        }

        return PagePath.NoActionNeeded;
    }

    private static string GetNextOfAmountOfPackaging(Page page)
    {
        var handlePackagingAnswers = GetActivityAnswers(page);

        var nextValue = page.NextValue();

        if (nextValue == PagePath.NoActionNeeded)
        {
            return nextValue;
        }

        if (IsNationData(handlePackagingAnswers) || (IsWasteCost(handlePackagingAnswers) && IsEndConsumerPackagingSeller(handlePackagingAnswers)))
        {
            return PagePath.WhatYouNeedToDo;
        }

        return nextValue;
    }

    private static string GetNextOfSingleUseContainersDrs(Page page)
    {
        if (!PlaceSingleUseContainersOnMarket(GetAnswers(page)))
        {
            return PagePath.AmountYouSupply;
        }

        return page.NextValue();
    }

    private static Dictionary<string, YesNoAnswer> GetActivityAnswers(Page page)
    {
        return GetAnswers(
                        page.FindPage(PagePath.OwnBrand),
                        page.FindPage(PagePath.UnbrandedPackaging),
                        page.FindPage(PagePath.ImportingProducts),
                        page.FindPage(PagePath.SupplyingEmptyPackaging),
                        page.FindPage(PagePath.HiringLoaning),
                        page.FindPage(PagePath.OnlineMarketplace),
                        page.FindPage(PagePath.SupplyingFilledPackaging));
    }

    private static Dictionary<string, YesNoAnswer> GetAnswers(params Page[] pages)
    {
        var answers = new Dictionary<string, YesNoAnswer>();

        foreach (var page in pages)
        {
            foreach (var question in page.Questions)
            {
                YesNoAnswer answer;
                var selectedValue = question.Options.Find(o => o.IsSelected == true)?.Value;

                if (question.QuestionType == QuestionType.CheckboxesNone)
                {
                    answer = selectedValue != "0" ? YesNoAnswer.Yes : YesNoAnswer.No;
                }
                else
                {
                    answer = selectedValue == "1" ? YesNoAnswer.Yes : YesNoAnswer.No;
                }

                answers.Add(question.Key, answer);
            }
        }

        return answers;
    }

    private static bool IsWasteCost(Dictionary<string, YesNoAnswer> answers)
    {
        return answers[QuestionKey.OwnBrand] == YesNoAnswer.Yes ||
               answers[QuestionKey.UnbrandedPackaging] == YesNoAnswer.Yes;
    }

    private static bool IsNationData(Dictionary<string, YesNoAnswer> answers)
    {
        bool isSupplierWithNationData =
            (answers[QuestionKey.OwnBrand] == YesNoAnswer.Yes ||
             answers[QuestionKey.UnbrandedPackaging] == YesNoAnswer.Yes) &&
            answers[QuestionKey.SupplyingFilledPackaging] == YesNoAnswer.Yes;

        if (isSupplierWithNationData)
        {
            return true;
        }

        return answers[QuestionKey.ImportingProducts] == YesNoAnswer.Yes ||
               answers[QuestionKey.OnlineMarketplace] == YesNoAnswer.Yes ||
               answers[QuestionKey.SellingEmptyPackaging] == YesNoAnswer.Yes ||
               answers[QuestionKey.HiringLoaning] == YesNoAnswer.Yes;
    }

    private static bool IsEndConsumerPackagingSeller(Dictionary<string, YesNoAnswer> answers)
    {
        return answers[QuestionKey.SupplyingFilledPackaging] == YesNoAnswer.Yes;
    }

    private static bool PlaceSingleUseContainersOnMarket(Dictionary<string, YesNoAnswer> answers)
    {
        return answers[QuestionKey.SingleUseContainersOnMarket] == YesNoAnswer.Yes;
    }
}