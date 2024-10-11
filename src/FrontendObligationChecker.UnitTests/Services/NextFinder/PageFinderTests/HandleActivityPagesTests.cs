using System.Reflection;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class HandleActivityPagesTests
{
    private static readonly int[] TestScenario1 =[0, 0, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario2 =[0, 0, 0, 0, 0, 1, 0];

    private static readonly int[] TestScenario3 =[1, 0, 0, 0, 0, 0, 1];
    private static readonly int[] TestScenario4 =[0, 1, 0, 0, 0, 0, 1];
    private static readonly int[] TestScenario5 =[0, 0, 1, 0, 0, 0, 1];
    private static readonly int[] TestScenario6 =[0, 0, 0, 1, 0, 0, 1];
    private static readonly int[] TestScenario7 =[0, 0, 0, 0, 1, 0, 1];
    private static readonly int[] TestScenario8 =[0, 0, 0, 0, 0, 1, 1];
    private static readonly int[] TestScenario9 =[1, 0, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario10 =[0, 1, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario11 =[0, 0, 1, 0, 0, 0, 0];
    private static readonly int[] TestScenario12 =[0, 0, 0, 1, 0, 0, 0];
    private static readonly int[] TestScenario13 =[0, 0, 0, 0, 1, 0, 0];
    private static readonly int[] TestScenario14 =[0, 0, 0, 0, 0, 1, 0];

    [TestMethod]
    [DataRow(nameof(TestScenario1))]
    public void NoActionNeeded_is_next_when_only_NO_answers_selected(string answers)
    {
        var answersValue = (int[])typeof(HandleActivityPagesTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

        Page page = PageHelper.GetLastActivityPageFromAnswers(answersValue);

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.NoActionNeeded, nextPath);
    }

    [TestMethod]
    [DataRow(nameof(TestScenario2))]
    public void PlaceDrinksOnMarket_is_next_when_seller_answer_is_YES(string answers)
    {
        var answersValue = (int[])typeof(HandleActivityPagesTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

        Page page = PageHelper.GetLastActivityPageFromAnswers(answersValue);

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.PlaceDrinksOnMarket, nextPath);
    }

    [TestMethod]
    [DataRow(nameof(TestScenario3))]
    [DataRow(nameof(TestScenario4))]
    [DataRow(nameof(TestScenario5))]
    [DataRow(nameof(TestScenario6))]
    [DataRow(nameof(TestScenario7))]
    [DataRow(nameof(TestScenario8))]
    [DataRow(nameof(TestScenario9))]
    [DataRow(nameof(TestScenario10))]
    [DataRow(nameof(TestScenario11))]
    [DataRow(nameof(TestScenario12))]
    [DataRow(nameof(TestScenario13))]
    [DataRow(nameof(TestScenario14))]
    public void PlaceDrinksOnMarket_is_next_when_one_or_more_answers_is_YES(string answers)
    {
        var answersValue = (int[])typeof(HandleActivityPagesTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        Page page = PageHelper.GetLastActivityPageFromAnswers(answersValue);

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.PlaceDrinksOnMarket, nextPath);
    }
}