namespace FrontendObligationChecker.Generators;

using Models.ObligationChecker;

public static class PageGenerator
{
    public static IEnumerable<Page> Create(string eprGuidanceUrl)
    {
        return new List<Page>
        {
            TypeOfOrganisationPage(),
            AnnualTurnoverPage(),
            AmountYouSupplyPage(),
            OwnBrandPage(QuestionKey.OwnBrand, PagePath.OwnBrand, PagePath.UnbrandedPackaging),
            UnbrandedPackagingPage(QuestionKey.UnbrandedPackaging, PagePath.UnbrandedPackaging, PagePath.ImportingProducts),
            ImportingProductsPage(QuestionKey.ImportingProducts , PagePath.ImportingProducts, PagePath.SupplyingEmptyPackaging),
            SellingEmptyPackagingPage(QuestionKey.SellingEmptyPackaging , PagePath.SupplyingEmptyPackaging, PagePath.HiringLoaning),
            HiringLoaningPage(QuestionKey.HiringLoaning , PagePath.HiringLoaning, PagePath.OnlineMarketplace),
            OnlineMarketplacePage(QuestionKey.OnlineMarketplace , PagePath.OnlineMarketplace, PagePath.SupplyingFilledPackaging),
            SupplyingFilledPackagingPage(QuestionKey.SupplyingFilledPackaging , PagePath.SupplyingFilledPackaging, PagePath.PlaceDrinksOnMarket),
            PlaceDrinksOnMarketPage(),
            MaterialsForDrinksContainersPage(),
            ContainerVolumePage(),
            NoActionNeededPage(),
            WhatYouNeedToDoPage(eprGuidanceUrl)
        };
    }

    private static Page TypeOfOrganisationPage()
    {
        return new Page()
        {
            Index = 10,
            Titles = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, "SingleQuestion.TypeOfOrganisation.Title"
                }
            },
            TitleCaption = "AboutYourOrganisation",
            Path = PagePath.TypeOfOrganisation,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.AnnualTurnover
                },
                {
                    OptionPath.Secondary, PagePath.AnnualTurnover
                },
            },
            IsPageHeading = true,
            AlternateRowTitle = "WhatYouNeedToDo.OrganisationTypeAlternateRowTitle",
            Questions = new List<Question>()
            {
                new()
                {
                    Key = QuestionKey.TypeOfOrganisation,
                    Description = "SingleQuestion.TypeOfOrganisation.QuestionDescription",
                    Summary = "SingleQuestion.TypeOfOrganisation.Summary",
                    Detail = "SingleQuestion.TypeOfOrganisation.Detail",
                    DetailPosition = DetailPosition.BelowQuestion,
                    Options = new List<Option>
                    {
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "SingleQuestion.TypeOfOrganisation.QuestionOptionLabel1",
                            Value = "parent"
                        },
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "SingleQuestion.TypeOfOrganisation.QuestionOptionLabel2",
                            Value = "subsidiary"
                        },
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "SingleQuestion.TypeOfOrganisation.QuestionOptionLabel3",
                            Value = "individual"
                        },
                    },
                    ErrorMessage = "SingleQuestion.TypeOfOrganisation.QuestionError"
                }
            }
        };
    }

    private static Page AnnualTurnoverPage()
    {
        return new Page()
        {
            Index = 20,
            Titles = new Dictionary<OptionPath, string>
            {
                {
                    OptionPath.Primary, "SingleQuestion.AnnualTurnover.Title1"
                },
                {
                    OptionPath.Secondary, "SingleQuestion.AnnualTurnover.Title2"
                }
            },
            TitleCaption = "AboutYourOrganisation",
            Path = PagePath.AnnualTurnover,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.NoActionNeeded
                },
                {
                    // OptionPath.Secondary, PagePath.OwnBrand
                    OptionPath.Secondary, PagePath.AmountYouSupply
                }
            },
            IsPageHeading = true,
            AlternateRowTitle = "WhatYouNeedToDo.AnnualTurnoverAlternateRowTitle",
            Questions = new List<Question>()
            {
                new()
                {
                    Key = QuestionKey.AnnualTurnover,
                    Options = new List<Option>()
                    {
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "SingleQuestion.AnnualTurnover.QuestionOptionLabel1",
                            Value = "1"
                        },
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "SingleQuestion.AnnualTurnover.QuestionOptionLabel2",
                            Value = "2"
                        },
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "SingleQuestion.AnnualTurnover.QuestionOptionLabel3",
                            Value = "3"
                        }
                    },
                    ErrorMessage = "SingleQuestion.AnnualTurnover.QuestionError",
                    Description = "SingleQuestion.AnnualTurnover.QuestionDescription"
                }
            },
            AdditionalDescriptions = new Dictionary<OptionPath, string>
            {
                {
                    OptionPath.Primary, "SingleQuestion.AnnualTurnover.AdditionalDescription1"
                }
            }
        };
    }

    private static Page AmountYouSupplyPage()
    {
        return new Page()
        {
            Index = 21,
            Titles = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Secondary, "AmountYouSupply.Title"
                },
                {
                    OptionPath.Primary, "AmountYouSupply.Title"
                }
            },
            TitleCaption = "AmountOfPackaging",
            Path = PagePath.AmountYouSupply,
            PageType = PageType.AmountYouSupply,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.NoActionNeeded
                },
                {
                    // OptionPath.Secondary, PagePath.WhatYouNeedToDo
                    OptionPath.Secondary, PagePath.OwnBrand
                }
            },
            IsPageHeading = false,
            AlternateRowTitle = "WhatYouNeedToDo.AmountYouSupplyAlternateRowTitle",
            Questions = new List<Question>()
            {
                new()
                {
                    Key = QuestionKey.AmountYouSupply,
                    Options = new List<Option>()
                    {
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "AmountYouSupply.Under25Tonnes",
                            Value = "1"
                        },
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "AmountYouSupply.25TonnesTo50Tonnes",
                            Value = "2"
                        },
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "AmountYouSupply.50TonnesOrMore",
                            Value = "3"
                        }
                    },
                    ErrorMessage = "AmountYouSupply.MultipleActivitiesQuestionError",
                    Title = "SingleQuestion.AmountYouSupply.QuestionTitle"
                }
            }
        };
    }

    private static Page MaterialsForDrinksContainersPage()
    {
        return new Page()
        {
            Index = 35,
            Titles = new Dictionary<OptionPath, string>
            {
                {
                    OptionPath.Primary, "SingleQuestion.MaterialsForDrinksContainers.Title"
                },
                {
                    OptionPath.Secondary, "SingleQuestion.MaterialsForDrinksContainers.Title"
                }
            },
            TitleCaption = "YourPackagingActivities",
            Path = PagePath.MaterialsForDrinksContainers,
            Paths = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, PagePath.ContainerVolume
                },
                {
                    // OptionPath.Secondary, PagePath.AmountYouSupply
                    OptionPath.Secondary, PagePath.WhatYouNeedToDo
                }
            },
            IsPageHeading = true,
            AlternateRowTitle = "WhatYouNeedToDo.MaterialsForDrinksContainersAlternateRowTitle",
            Questions = new List<Question>()
            {
                new()
                {
                    QuestionType = QuestionType.CheckboxesNone,
                    Key = QuestionKey.MaterialsForDrinksContainers,
                    Options = new List<Option>()
                    {
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "SingleQuestion.MaterialsForDrinksContainers.PlasticBottles",
                            Value = "1"
                        },
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "SingleQuestion.MaterialsForDrinksContainers.GlassBottles",
                            Value = "2"
                        },
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "SingleQuestion.MaterialsForDrinksContainers.SteelCans",
                            Value = "3"
                        },
                        new()
                        {
                            Next = OptionPath.Primary,
                            Title = "SingleQuestion.MaterialsForDrinksContainers.AluminiumCans",
                            Value = "4"
                        }
                        ,
                        new()
                        {
                            Next = OptionPath.Secondary,
                            Title = "SingleQuestion.MaterialsForDrinksContainers.None",
                            Value = "0"
                        }
                    },
                    ErrorMessage = "SingleQuestion.MaterialsForDrinksContainers.QuestionError",
                    Title = "SingleQuestion.MaterialsForDrinksContainers.QuestionTitle",
                    Description = "SingleQuestion.MaterialsForDrinksContainers.QuestionDescription"
                }
            }
        };
    }

    private static Page NoActionNeededPage()
    {
        return new Page()
        {
            Index = 50,
            Titles = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, "NoActionNeeded.Title"
                },
                {
                    OptionPath.Secondary, "NoActionNeeded.Title"
                }
            },
            Contents = GetContents(),
            Path = PagePath.NoActionNeeded,
            PageType = PageType.NoActionNeeded,
            IsBackButtonHidden = true
        };
    }

    private static Page WhatYouNeedToDoPage(string guidanceUrl)
    {
        return new Page()
        {
            Index = 60,
            Titles = new Dictionary<OptionPath, string>()
            {
                {
                    OptionPath.Primary, "WhatYouNeedToDo.Title"
                },
                {
                    OptionPath.Secondary, "WhatYouNeedToDo.Title"
                }
            },
            Contents = GetWhatYouNeedToDoContents(guidanceUrl),
            Path = PagePath.WhatYouNeedToDo,
            PageType = PageType.WhatYouNeedToDo,
            IsBackButtonHidden = true
        };
    }

    private static Page PlaceDrinksOnMarketPage()
    {
        return new Page()
        {
            Index = 33,
            Titles = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, "SingleQuestion.PlaceDrinksOnMarket.Title" },
                { OptionPath.Secondary, "SingleQuestion.PlaceDrinksOnMarket.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.PlaceDrinksOnMarketAlternateRowTitle",
            Path = PagePath.PlaceDrinksOnMarket,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, PagePath.MaterialsForDrinksContainers },
                 { OptionPath.Secondary, PagePath.WhatYouNeedToDo }
                // { OptionPath.Secondary, PagePath.AmountYouSupply }
            },
            IsPageHeading = true,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = QuestionKey.SingleUseContainersOnMarket,
                    Summary = "SingleQuestion.PlaceDrinksOnMarket.ScotlandSummary",
                    Detail = "SingleQuestion.PlaceDrinksOnMarket.ScotlandDetail",
                    DetailPosition = DetailPosition.BelowQuestion,
                    Options = BooleanOptions,
                    ErrorMessage = "SingleQuestion.PlaceDrinksOnMarket.Error",
                }
            }
        };
    }

    private static Page SupplyingFilledPackagingPage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 28,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.SupplyingFilledPackaging.Title" },
                { OptionPath.Secondary, "SingleQuestion.SupplyingFilledPackaging.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.SupplyingFilledPackaging.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath }
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.SupplyingFilledPackaging.QuestionTitle",
                    AlternateTitle = "SingleQuestion.SupplyingFilledPackaging.AlternateTitle",
                    ErrorMessage = "SingleQuestion.SupplyingFilledPackaging.QuestionError",
                }
            },
            AlternateTitleFirstParagraph = "SingleQuestion.SupplyingFilledPackaging.SupplierConsumerDescriptions",
            PageGroup = PageGroup.Activity,
        };
    }

    private static Page OwnBrandPage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 22,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.OwnBrand.Title" },
                { OptionPath.Secondary, "SingleQuestion.OwnBrand.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.OwnBrand.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath } // Page path was previously set to no-action-needed i.e. PagePath.NoActionNeeded.
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.OwnBrand.QuestionTitle",
                    AlternateTitle = "SingleQuestion.OwnBrand.AlternateTitle",
                    ErrorMessage = "SingleQuestion.OwnBrand.QuestionError",
                }
            },
            AlternateTitleSubContents = new List<string>
            {
                "SingleQuestion.OwnBrand.AnotherOrganisationTasks"
            },
            AlternateTitleFirstParagraph = "SingleQuestion.OwnBrand.BrandDescriptions",
            PageGroup=PageGroup.Activity,
        };
    }

    private static Page UnbrandedPackagingPage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 23,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.UnbrandedPackaging.Title" },
                { OptionPath.Secondary, "SingleQuestion.UnbrandedPackaging.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.UnbrandedPackaging.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath }
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.UnbrandedPackaging.QuestionTitle",
                    AlternateTitle = "SingleQuestion.UnbrandedPackaging.AlternateTitle",
                    ErrorMessage = "SingleQuestion.UnbrandedPackaging.QuestionError",
                }
            },
            AlternateTitleFirstParagraph = "SingleQuestion.UnbrandedPackaging.PlacingGoodsDescriptions",
            PageGroup = PageGroup.Activity,
        };
    }

    private static Page ImportingProductsPage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 24,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.ImportingProducts.Title" },
                { OptionPath.Secondary, "SingleQuestion.ImportingProducts.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.ImportingProducts.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath }
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.ImportingProducts.QuestionTitle",
                    AlternateTitle = "SingleQuestion.ImportingProducts.AlternateTitle",
                    AmountHandlePageText = "SingleQuestion.ImportingProducts.AmountHandlePageText",
                    ErrorMessage = "SingleQuestion.ImportingProducts.QuestionError",
                }
            },
            AlternateTitleFirstParagraph = "SingleQuestion.ImportingProducts.PackagingImportDescriptions",
            PageGroup = PageGroup.Activity,
        };
    }

    private static Page SellingEmptyPackagingPage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 25,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.SellingEmptyPackaging.Title" },
                { OptionPath.Secondary, "SingleQuestion.SellingEmptyPackaging.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.SellingEmptyPackaging.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath }
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.SellingEmptyPackaging.QuestionTitle",
                    AlternateTitle = "SingleQuestion.SellingEmptyPackaging.AlternateTitle",
                    AmountHandlePageText = "SingleQuestion.SellingEmptyPackaging.AmountHandlePageText",
                    ErrorMessage = "SingleQuestion.SellingEmptyPackaging.QuestionError",
                }
            },
            AlternateTitleFirstParagraph = "SingleQuestion.SellingEmptyPackaging.HigherThresholdDescriptions",
            PageGroup = PageGroup.Activity,
        };
    }

    private static Page HiringLoaningPage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 26,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.HiringLoaning.Title" },
                { OptionPath.Secondary, "SingleQuestion.HiringLoaning.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.HiringLoaning.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath }
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.HiringLoaning.QuestionTitle",
                    AlternateTitle = "SingleQuestion.HiringLoaning.AlternateTitle",
                    ErrorMessage = "SingleQuestion.HiringLoaning.QuestionError",
                }
            },
            AlternateTitleFirstParagraph = "SingleQuestion.HiringLoaning.PackagingHireLoanDescriptions",
            PageGroup = PageGroup.Activity,
        };
    }

    private static Page OnlineMarketplacePage(string questionKey, string path, string nextPath)
    {
        return new Page()
        {
            Index = 27,
            Titles = new Dictionary<OptionPath, string>
            {
                { OptionPath.Primary, "SingleQuestion.OnlineMarketplace.Title" },
                { OptionPath.Secondary, "SingleQuestion.OnlineMarketplace.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.OnlineMarketplace.AlternateRowTitle",
            Path = path,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, nextPath },
                { OptionPath.Secondary, nextPath }
            },
            IsPageHeading = false,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = questionKey,
                    Options = BooleanOptions,
                    Title = "SingleQuestion.OnlineMarketplace.QuestionTitle",
                    AlternateTitle = "SingleQuestion.OnlineMarketplace.AlternateTitle",
                    ErrorMessage = "SingleQuestion.OnlineMarketplace.QuestionError",
                }
            },
            AlternateTitleFirstParagraph = "SingleQuestion.OnlineMarketplace.OnlineMarketplaceDescriptions",
            PageGroup = PageGroup.Activity,
        };
    }

    private static Page ContainerVolumePage()
    {
        return new Page()
        {
            Index = 37,
            Titles = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, "SingleQuestion.ContainerVolume.Title" },
                { OptionPath.Secondary, "SingleQuestion.ContainerVolume.Title" }
            },
            TitleCaption = "YourPackagingActivities",
            AlternateRowTitle = "WhatYouNeedToDo.ContainerVolumeAlternateRowTitle",
            Path = PagePath.ContainerVolume,
            Paths = new Dictionary<OptionPath, string>()
            {
                { OptionPath.Primary, PagePath.MaterialsForDrinksContainers },
                { OptionPath.Secondary, PagePath.WhatYouNeedToDo }
                // { OptionPath.Secondary, PagePath.AmountYouSupply }
            },
            IsPageHeading = true,
            Questions = new List<Question>()
            {
                new()
                {
                    Key = QuestionKey.ContainerVolume,
                    Options = BooleanOptions,
                    Description = "SingleQuestion.ContainerVolume.QuestionDescription",
                    ErrorMessage = "SingleQuestion.ContainerVolume.QuestionError",
                }
            }
        };
    }

    private static List<Option> BooleanOptions =>
        new()
        {
            new Option
            {
                Next = OptionPath.Primary,
                Title = "question_option_yes",
                Value = YesNo.Yes
            },
            new Option
            {
                Next = OptionPath.Secondary,
                Title = "question_option_no",
                Value = YesNo.No
            }
        };

    private static Dictionary<string, Content> GetContents()
    {
        var contents = new Dictionary<string, Content>
        {
            {
                PagePath.AnnualTurnover, GetAnnualTurnoverContents()
            },
            {
                PagePath.SupplyingFilledPackaging, GetHandleSupplyPackagingContents()
            },
            {
                PagePath.AmountYouSupply, GetAmountYouSupplyContents()
            },
            {
                PagePath.OwnBrand, GetOwnBrandContents()
            }
        };

        return contents;
    }

    private static Dictionary<string, Content> GetWhatYouNeedToDoContents(string guidanceUrl)
    {
        return new Dictionary<string, Content>()
        {
            {
                PagePath.WhatYouNeedToDo, new Content
                {
                    ContentItems = new List<ContentItem>()
                    {
                        new(ContentType.Banner, "WhatYouNeedToDo.ContentBanner", CompanySize.All),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph1Small", CompanySize.Small, SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph1SmallSubsidiary", CompanySize.Small, SellerType.NotSellerOnly, associationType: AssociationType.Subsidiary),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph1Large", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Print, new List<string>() { "WhatYouNeedToDo.ContentPrint1", "WhatYouNeedToDo.ContentPrint2" }, CompanySize.All),
                        new(ContentType.Heading, "WhatYouNeedToDo.ContentHeading1", CompanySize.All),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph2Small", CompanySize.Small, SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph2Large", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Heading, "WhatYouNeedToDo.WhenToCollectAndReportHeader", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.WhenToCollectAndReportContent", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Inset, new List<string>() { "WhatYouNeedToDo.ContentInset1Small" }, CompanySize.Small, SellerType.NotSellerOnly),
                        new(ContentType.Inset, new List<string>() { "WhatYouNeedToDo.ContentInset1Large", "WhatYouNeedToDo.ContentInset2Large" }, CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph3Large", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.UnorderedList, new List<string>() { "WhatYouNeedToDo.ContentUoList1A", "WhatYouNeedToDo.ContentUoList1B" }, CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Heading3, "WhatYouNeedToDo.ContentHeading2Large", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph4Large", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph5Large", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Details, "WhatYouNeedToDo.ContentDetailsPrnPerns", CompanySize.Large, SellerType.NotSellerOnly),
                        new(ContentType.Heading3, "WhatYouNeedToDo.ContentNationDataHeading", CompanySize.All, requiresNationData: true, sellerType: SellerType.NotSellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentNationDataParagraph1", CompanySize.All, requiresNationData: true),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentNationDataParagraph2", CompanySize.All, requiresNationData: true),
                        new(ContentType.Inset, new List<string>() { "WhatYouNeedToDo.ContentNationDataInset1" }, CompanySize.All, SellerType.NotSellerOnly, requiresNationData: true),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentNationDataParagraph3", CompanySize.All, SellerType.SellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.SupplyingPackagingContent", CompanySize.All, SellerType.SellerOnly),
                        new(ContentType.Heading, "WhatYouNeedToDo.ContentHeading4", CompanySize.Large),
                        new(ContentType.Heading, "WhatYouNeedToDo.ContentHeading4", CompanySize.Small, SellerType.SellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.ContentParagraph2Seller", CompanySize.All, SellerType.SellerOnly),
                        new(ContentType.Paragraph, "WhatYouNeedToDo.WhatYouShouldDoNextParagraph", CompanySize.Large, SellerType.NotSellerOnly),
                        new Hyperlink(guidanceUrl, "WhatYouNeedToDo.WhatYouShouldDoNextLinkDescription","find-out-more", CompanySize.Large),
                        new Hyperlink(guidanceUrl, "WhatYouNeedToDo.WhatYouShouldDoNextLinkDescription","find-out-more", CompanySize.Small, SellerType.SellerOnly),
                        new(ContentType.Inset, new List<string>() { "WhatYouNeedToDo.ContentNationDataInset1" }, CompanySize.All, SellerType.SellerOnly, requiresNationData: true),
                        new(ContentType.SeparatorLine),
                        new(ContentType.Heading, "WhatYouNeedToDo.ContentHeading5", CompanySize.All),
                    }
                }
            }
        };
    }

    private static Content GetAnnualTurnoverContents()
    {
        return new Content
        {
            ContentItems = new List<ContentItem>
            {
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescriptionOrganization", associationType: AssociationType.Organisation),
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescriptionParent", associationType: AssociationType.Parent),
                new(ContentType.Heading, "NoActionNeeded.AnnualTurnoverTitle1", associationType: AssociationType.All),
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescription2", associationType: AssociationType.Subsidiary),
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescription2", associationType: AssociationType.Individual),
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescription2Parent", associationType: AssociationType.Parent),
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescription3Parent", associationType: AssociationType.Parent),
                new(ContentType.Heading, "NoActionNeeded.AnnualTurnoverTitle2", associationType: AssociationType.Subsidiary),
                new(ContentType.Paragraph, "NoActionNeeded.AnnualTurnoverDescription3", associationType: AssociationType.Subsidiary),
            }
        };
    }

    private static Content GetHandleSupplyPackagingContents()
    {
        return new Content
        {
            ContentItems = new List<ContentItem>
            {
                new(ContentType.Paragraph, "NoActionNeeded.HandleSupplyPackagingDescription1"),
                new(ContentType.Heading, "NoActionNeeded.HandleSupplyPackagingTitle1", associationType: AssociationType.All),
                new(ContentType.Paragraph, "NoActionNeeded.HandleSupplyPackagingDescription2"),
            }
        };
    }

    private static Content GetAmountYouSupplyContents()
    {
        return new Content
        {
            ContentItems = new List<ContentItem>()
            {
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription1", associationType: AssociationType.Subsidiary),
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription1", associationType: AssociationType.Individual),
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription1Parent", associationType: AssociationType.Parent),
                new(ContentType.Heading, "NoActionNeeded.AmountYouSupplyTitle1"),
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription2", associationType: AssociationType.Subsidiary),
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription2", associationType: AssociationType.Individual),
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription2Parent", associationType: AssociationType.Parent),
                new(ContentType.Heading, "NoActionNeeded.AmountYouSupplyTitle2", associationType: AssociationType.Subsidiary),
                new(ContentType.Paragraph, "NoActionNeeded.AmountYouSupplyDescription3", associationType: AssociationType.Subsidiary)
            }
        };
    }

    private static Content GetOwnBrandContents()
    {
        return new Content
        {
            ContentItems = new List<ContentItem>()
            {
               new(ContentType.Heading, "NoActionNeeded.OwnBrandTitle", associationType: AssociationType.Parent),
               new(ContentType.Paragraph, "NoActionNeeded.OwnBrandDescription1", associationType: AssociationType.Parent)
            }

        };
    }
}