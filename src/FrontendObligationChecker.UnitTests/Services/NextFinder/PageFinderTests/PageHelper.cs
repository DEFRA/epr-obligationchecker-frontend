using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder.PageFinderTests
{
    public static class PageHelper
    {
        public struct PageDescriptor
        {
            public string Path { get; set; }

            public string NewPathPrimary { get; set; }

            public string NextPathSecondary { get; set; }

            public string QuestionKey { get; set; }
        }

        private static readonly List<PageDescriptor> PageDescriptors = new List<PageDescriptor>
        {
            new PageDescriptor
            {
                Path = PagePath.OwnBrand,
                QuestionKey= QuestionKey.OwnBrand,
                NewPathPrimary=PagePath.UnbrandedPackaging,
                NextPathSecondary=PagePath.UnbrandedPackaging
            },
            new PageDescriptor
            {
                Path = PagePath.UnbrandedPackaging,
                QuestionKey= QuestionKey.UnbrandedPackaging,
                NewPathPrimary=PagePath.ImportingProducts,
                NextPathSecondary=PagePath.ImportingProducts
            },
            new PageDescriptor
            {
                Path = PagePath.ImportingProducts,
                QuestionKey= QuestionKey.ImportingProducts,
                NewPathPrimary=PagePath.SupplyingEmptyPackaging,
                NextPathSecondary=PagePath.SupplyingEmptyPackaging
            },
            new PageDescriptor
            {
                Path = PagePath.SupplyingEmptyPackaging,
                QuestionKey= QuestionKey.SellingEmptyPackaging,
                NewPathPrimary=PagePath.HiringLoaning,
                NextPathSecondary=PagePath.HiringLoaning
            },
            new PageDescriptor
            {
                Path = PagePath.HiringLoaning,
                QuestionKey= QuestionKey.HiringLoaning,
                NewPathPrimary=PagePath.OnlineMarketplace,
                NextPathSecondary=PagePath.OnlineMarketplace
            },
            new PageDescriptor
            {
                Path = PagePath.OnlineMarketplace,
                QuestionKey= QuestionKey.OnlineMarketplace,
                NewPathPrimary=PagePath.SupplyingFilledPackaging,
                NextPathSecondary=PagePath.SupplyingFilledPackaging
            },
            new PageDescriptor
            {
                Path = PagePath.SupplyingFilledPackaging,
                QuestionKey= QuestionKey.SupplyingFilledPackaging,
                NewPathPrimary=PagePath.PlaceDrinksOnMarket,
                NextPathSecondary=PagePath.PlaceDrinksOnMarket
            },
        };

        public static Page GetLastActivityPageFromAnswers(IReadOnlyList<int> answers)
        {
            Page? lastPage = null;
            var pages = new List<Page>();

            for (int i = 0; i < answers.Count; i++)
            {
                var pageDescriptor = PageDescriptors[i];
                lastPage = BuildPage(pageDescriptor, answers[i], lastPage);
                pages.Add(lastPage);
            }

            lastPage.SessionPages = pages;

            return lastPage;
        }

        private static Page BuildPage(PageDescriptor pageDescriptor, int answer, Page? previousPage)
        {
            return new Page
            {
                PreviousPage = previousPage,
                Path = pageDescriptor.Path,
                Paths = new Dictionary<OptionPath, string>()
                {
                    { OptionPath.Primary, pageDescriptor.NewPathPrimary },
                    { OptionPath.Secondary, pageDescriptor.NextPathSecondary }
                },
                Questions = new List<Question>
                {
                    new Question
                    {
                        Key = pageDescriptor.QuestionKey,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                IsSelected = true,
                                Value = answer == 1 ? YesNo.Yes : YesNo.No
                            }
                        }
                    }
                }
            };
        }
    }
}