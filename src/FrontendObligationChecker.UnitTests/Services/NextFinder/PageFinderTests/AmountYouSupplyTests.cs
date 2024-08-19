using System.Reflection;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests;

[TestClass]
public class AmountYouSupplyTests
{
    private static readonly int[] TestScenario1 =[0, 0, 1, 0, 0, 0, 0];
    private static readonly int[] TestScenario2 =[0, 0, 0, 1, 0, 0, 0];
    private static readonly int[] TestScenario3 =[0, 0, 0, 0, 1, 0, 0];
    private static readonly int[] TestScenario4 =[0, 0, 0, 0, 0, 1, 0];
    private static readonly int[] TestScenario5 =[0, 0, 1, 0, 0, 0, 1];
    private static readonly int[] TestScenario6 =[0, 0, 0, 1, 0, 0, 1];
    private static readonly int[] TestScenario7 =[0, 0, 0, 0, 1, 0, 1];
    private static readonly int[] TestScenario8 =[0, 0, 0, 0, 0, 1, 1];
    private static readonly int[] TestScenario9 =[1, 0, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario10 =[0, 1, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario11 =[1, 1, 1, 1, 1, 1, 1];
    private static readonly int[] TestScenario12 =[1, 0, 1, 0, 1, 0, 1];
    private static readonly int[] TestScenario13 =[0, 1, 0, 1, 0, 1, 0];
    private static readonly int[] TestScenario14 =[0, 0, 0, 0, 0, 0, 0];
    private static readonly int[] TestScenario15 =[1, 1, 0, 0, 1, 1, 1];
    private static readonly int[] TestScenario16 =[0, 0, 1, 1, 0, 0, 0];
    private static readonly int[] TestScenario17 =[0, 0, 0, 0, 1, 1, 1];
    private static readonly int[] TestScenario18 =[1, 1, 1, 0, 0, 0, 0];
    private static readonly string[] TestScenario19 =["Under 25 tonnes", "1"];
    private static readonly string[] TestScenario20 =["25 tonnes to 50 tonnes", "2"];
    private static readonly string[] TestScenario21 =["More than 50 tonnes", "3"];

    [TestMethod]
    [DataRow(nameof(TestScenario1))]
    [DataRow(nameof(TestScenario2))]
    [DataRow(nameof(TestScenario3))]
    [DataRow(nameof(TestScenario4))]
    [DataRow(nameof(TestScenario5))]
    [DataRow(nameof(TestScenario6))]
    [DataRow(nameof(TestScenario7))]
    [DataRow(nameof(TestScenario8))]
    public void NationData_page_is_next_when_any_of_nation_data_answers_were_YES(string answers)
    {
        var answersValue = (int[])typeof(AmountYouSupplyTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answersValue);
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
    [DataRow(nameof(TestScenario9))]
    [DataRow(nameof(TestScenario10))]
    public void WhatYouNeedToDo_page_is_next_when_any_of_waste_answers_were_YES_and_all_of_nation_data_answers_and_seller_were_NO(string answers)
    {
        var answersValue = (int[])typeof(AmountYouSupplyTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answersValue);
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
    [DataRow(nameof(TestScenario1))]
    [DataRow(nameof(TestScenario2))]
    [DataRow(nameof(TestScenario3))]
    [DataRow(nameof(TestScenario4))]
    [DataRow(nameof(TestScenario5))]
    [DataRow(nameof(TestScenario6))]
    [DataRow(nameof(TestScenario7))]
    [DataRow(nameof(TestScenario8))]
    // Outcome_page_is_next_when_any_of_waste_answers_were_YES_and_all_of_nation_data_answers_and_seller_were_NO() test DataRows:
    [DataRow(nameof(TestScenario9))]
    [DataRow(nameof(TestScenario10))]
    // All options selected on handle packaging page
    [DataRow(nameof(TestScenario11))]
    // Random selection
    [DataRow(nameof(TestScenario12))]
    [DataRow(nameof(TestScenario13))]
    [DataRow(nameof(TestScenario14))]

    [DataRow(nameof(TestScenario15))]
    [DataRow(nameof(TestScenario16))]
    [DataRow(nameof(TestScenario17))]
    [DataRow(nameof(TestScenario18))]
    public void NoActionNeeded_is_next_for_small_tonnage(string answers)
    {
        var answersValue = (int[])typeof(AmountYouSupplyTests).GetField(answers, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var lastActivityPage = PageHelper.GetLastActivityPageFromAnswers(answersValue);
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
    [DataRow(nameof(TestScenario19))]
    [DataRow(nameof(TestScenario20))]
    [DataRow(nameof(TestScenario21))]
    public void NoActionNeeded_is_next_for_all_tonnage_when_no_to_all_packaging_activities(string tonnage)
    {
        var tonnageValue = (string[])typeof(AmountYouSupplyTests).GetField(tonnage, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

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
                            Title = tonnageValue[0],
                            Value = tonnageValue[1],
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