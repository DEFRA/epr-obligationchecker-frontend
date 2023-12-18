using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class HandleActivityPagesTests
{
    [TestMethod]
    [DataRow(new[] { 0, 0, 0, 0, 0, 0, 0 })]
    public void NoActionNeeded_is_next_when_only_NO_answers_selected(int[] answers)
    {
        Page page = PageHelper.GetLastActivityPageFromAnswers(answers);

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.NoActionNeeded, nextPath);
    }

    [TestMethod]
    [DataRow(new[] { 0, 0, 0, 0, 0, 0, 1 })]
    public void PlaceDrinksOnMarket_is_next_when_seller_answer_is_YES(int[] answers)
    {
        Page page = PageHelper.GetLastActivityPageFromAnswers(answers);

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.PlaceDrinksOnMarket, nextPath);
    }

    [TestMethod]
    [DataRow(new[] { 1, 0, 0, 0, 0, 0, 1 })]
    [DataRow(new[] { 0, 1, 0, 0, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 1 })]
    [DataRow(new[] { 1, 0, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 1, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 0 })]
    public void PlaceDrinksOnMarket_is_next_when_one_or_more_answers_is_YES(int[] answers)
    {
        Page page = PageHelper.GetLastActivityPageFromAnswers(answers);

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.PlaceDrinksOnMarket, nextPath);
    }
}