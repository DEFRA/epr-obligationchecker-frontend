using System.Reflection;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class PlaceDrinksOnMarketTests
{
    private static readonly int[] TestScenario1 =[0, 0, 0, 0, 0, 0, 1];
    private static readonly int[] TestScenario2 =[0, 0, 0, 1, 0, 0, 0];
    private static readonly int[] TestScenario3 =[0, 0, 0, 0, 1, 0, 0];
    private static readonly int[] TestScenario4 =[0, 0, 0, 0, 0, 1, 0];
    private static readonly int[] TestScenario5 =[0, 0, 1, 0, 0, 0, 1];
    private static readonly int[] TestScenario6 =[0, 0, 0, 1, 0, 0, 1];
    private static readonly int[] TestScenario7 =[0, 0, 0, 0, 1, 0, 1];
    private static readonly int[] TestScenario8 =[0, 0, 0, 0, 0, 1, 1];
    private static readonly int[] TestScenario9 =[1, 0, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario10 =[0, 1, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario11 =[0, 0, 1, 0, 0, 0, 0];

    [TestMethod]
    [DataRow(nameof(TestScenario1))]
    public void WhatYouNeedToDo_is_next_when_only_seller_answer_was_YES(string answers)
    {
        var answersValue = (int[])typeof(PlaceDrinksOnMarketTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var page = SetUpPage(answersValue, "0");

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.WhatYouNeedToDo, nextPath);
    }

    [TestMethod]
    [DataRow(nameof(TestScenario2))]
    [DataRow(nameof(TestScenario3))]
    [DataRow(nameof(TestScenario4))]
    [DataRow(nameof(TestScenario9))]
    [DataRow(nameof(TestScenario10))]
    [DataRow(nameof(TestScenario11))]
    public void AmountYouSupply_is_next_when_seller_answer_was_NO_and_some_other_answers_were_Yes(string answers)
    {
        var answersValue = (int[])typeof(PlaceDrinksOnMarketTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var page = SetUpPage(answersValue, "0");

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.WhatYouNeedToDo, nextPath);
    }

    [TestMethod]
    [DataRow(nameof(TestScenario1))]
    [DataRow(nameof(TestScenario2))]
    [DataRow(nameof(TestScenario3))]
    [DataRow(nameof(TestScenario4))]
    [DataRow(nameof(TestScenario9))]
    [DataRow(nameof(TestScenario10))]
    [DataRow(nameof(TestScenario11))]
    public void MaterialsForDrinksContainers_is_next_when_single_use_containers_are_placed_on_market(string answers)
    {
        var answersValue = (int[])typeof(PlaceDrinksOnMarketTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var page = SetUpPage(answersValue, "1");

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