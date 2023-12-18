using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class PlaceDrinksOnMarketTests
{
    [TestMethod]
    [DataRow(new[] { 0, 0, 0, 0, 0, 0, 1 })]
    public void WhatYouNeedToDo_is_next_when_only_seller_answer_was_YES(int[] answers)
    {
        var page = SetUpPage(answers, "0");

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.AmountYouSupply, nextPath);
    }

    [TestMethod]
    [DataRow(new[] { 1, 0, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 1, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 0 })]
    public void AmountYouSupply_is_next_when_seller_answer_was_NO_and_some_other_answers_were_Yes(int[] answers)
    {
        var page = SetUpPage(answers, "0");

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.AmountYouSupply, nextPath);
    }

    [TestMethod]
    [DataRow(new[] { 1, 0, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 1, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 0, 1 })]
    public void MaterialsForDrinksContainers_is_next_when_single_use_containers_are_placed_on_market(int[] answers)
    {
        var page = SetUpPage(answers, "1");

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.MaterialsForDrinksContainers, nextPath);
    }

    private static Page SetUpPage(int[] answers, string value)
    {
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answers);
        return new Page()
        {
            Path = PagePath.PlaceDrinksOnMarket,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, PagePath.MaterialsForDrinksContainers },
                { OptionPath.Secondary, PagePath.AmountYouSupply }
            },
            PreviousPage = lastActivityPage,
            SessionPages = lastActivityPage.SessionPages,
            Questions = new List<Question>()
            {
                new Question()
                {
                    Key = QuestionKey.SingleUseContainersOnMarket,
                    Options = new List<Option>()
                    {
                        new()
                        {
                            Value = value,
                            IsSelected = true
                        }
                    }
                }
            }
        };
    }
}