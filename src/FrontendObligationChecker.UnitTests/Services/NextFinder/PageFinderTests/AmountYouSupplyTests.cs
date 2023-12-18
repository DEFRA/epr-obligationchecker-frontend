using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class AmountYouSupplyTests
{
    [TestMethod]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 0 })]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 1 })]
    public void NationData_page_is_next_when_any_of_nation_data_answers_were_YES(int[] answers)
    {
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answers);
        var page = new Page()
        {
            Path = PagePath.AmountYouSupply,
            PreviousPage = lastActivityPage,
            SessionPages = lastActivityPage.SessionPages,
            Questions = new List<Question>()
            {
                new Question()
                {
                    Options = new List<Option>()
                    {
                        new Option()
                        {
                            Next = OptionPath.Secondary,
                            Title = "25 tonnes to 50 tonnes",
                            Value = "2",
                            IsSelected = true
                        }
                    }
                }
            },
        };

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.WhatYouNeedToDo, nextPath);
    }

    [TestMethod]
    [DataRow(new[] { 1, 0, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 1, 0, 0, 0, 0, 0 })]
    public void WhatYouNeedToDo_page_is_next_when_any_of_waste_answers_were_YES_and_all_of_nation_data_answers_and_seller_were_NO(int[] answers)
    {
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answers);
        var page = new Page()
        {
            Path = PagePath.AmountYouSupply,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.NoActionNeeded
                },
                {
                    OptionPath.Secondary, PagePath.WhatYouNeedToDo
                }
            },
            PreviousPage = lastActivityPage,
            SessionPages = lastActivityPage.SessionPages,
            Questions = new List<Question>()
            {
                new Question()
                {
                    Options = new List<Option>()
                    {
                        new Option()
                        {
                            Next = OptionPath.Secondary,
                            Title = "25 tonnes to 50 tonnes",
                            Value = "2",
                            IsSelected = true
                        }
                    }
                }
            }
        };

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.WhatYouNeedToDo, nextPath);
    }

    [TestMethod]
    // Interruption_page_is_next_when_any_of_nation_data_answers_were_YES() test DataRows:
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 0 })]
    [DataRow(new[] { 0, 0, 1, 0, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 1, 0, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 0, 1 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 1, 1 })]
    // Outcome_page_is_next_when_any_of_waste_answers_were_YES_and_all_of_nation_data_answers_and_seller_were_NO() test DataRows:
    [DataRow(new[] { 1, 0, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 0, 1, 0, 0, 0, 0, 0 })]
    // All options selected on handle packaging page
    [DataRow(new[] { 1, 1, 1, 1, 1, 1, 1 })]
    // Random selection
    [DataRow(new[] { 1, 0, 1, 0, 1, 0, 1 })]
    [DataRow(new[] { 0, 1, 0, 1, 0, 1, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 0, 0, 0 })]
    [DataRow(new[] { 1, 1, 0, 0, 1, 1, 1 })]
    [DataRow(new[] { 0, 0, 1, 1, 0, 0, 0 })]
    [DataRow(new[] { 0, 0, 0, 0, 1, 1, 1 })]
    [DataRow(new[] { 1, 1, 1, 0, 0, 0, 0 })]
    public void NoActionNeeded_is_next_for_small_tonnage(int[] answers)
    {
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answers);
        var page = new Page()
        {
            Path = PagePath.AmountYouSupply,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.NoActionNeeded
                },
                {
                    OptionPath.Secondary, PagePath.WhatYouNeedToDo
                }
            },
            PreviousPage = lastActivityPage,
            SessionPages = lastActivityPage.SessionPages,
            Questions = new List<Question>()
            {
                new Question()
                {
                    Options = new List<Option>()
                    {
                        new Option()
                        {
                            Next = OptionPath.Primary,
                            Title = "Under 25 tonnes",
                            Value = "1",
                            IsSelected = true
                        }
                    }
                }
            }
        };

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.NoActionNeeded, nextPath);
    }

    [TestMethod]
    [DataRow(new[] { "Under 25 tonnes", "1" })]
    [DataRow(new[] { "25 tonnes to 50 tonnes", "2" })]
    [DataRow(new[] { "More than 50 tonnes", "3" })]
    public void NoActionNeeded_is_next_for_all_tonnage_when_no_to_all_packaging_activities(string[] tonnage)
    {
        var answers = new[] { 0, 0, 0, 0, 0, 0, 0 };
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answers);
        var page = new Page()
        {
            Path = PagePath.AmountYouSupply,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.NoActionNeeded
                },
                {
                    OptionPath.Secondary, PagePath.WhatYouNeedToDo
                }
            },
            PreviousPage = lastActivityPage,
            SessionPages = lastActivityPage.SessionPages,
            Questions = new List<Question>()
            {
                new Question()
                {
                    Options = new List<Option>()
                    {
                        new Option()
                        {
                            Next = OptionPath.Primary,
                            Title = tonnage[0],
                            Value = tonnage[1],
                            IsSelected = true
                        }
                    }
                }
            }
        };

        // Act
        var nextPath = PageFinder.GetNextPath(page);

        Assert.AreEqual(PagePath.NoActionNeeded, nextPath);
    }
}