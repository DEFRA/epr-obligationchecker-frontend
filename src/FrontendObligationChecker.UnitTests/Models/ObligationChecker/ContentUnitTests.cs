namespace FrontendObligationChecker.UnitTests.Models.ObligationChecker;

using FrontendObligationChecker.Models.ObligationChecker;

[TestClass]
public class ContentUnitTests
{
    [TestMethod]
    public void GetRelatedContentItems_ReturnsAll_WhenAssociationTypeIsNotSet()
    {
        // Arrange
        var items = new List<ContentItem>
        {
            new(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.All) { AssociationType = AssociationType.Individual },
            new(ContentType.Paragraph, companySize: CompanySize.All, sellerType: SellerType.All) { AssociationType = AssociationType.Subsidiary }
        };

        var content = new Content { ContentItems = items };

        // Act
        var result = content.GetRelatedContentItems(AssociationType.NotSet);

        // Assert
        CollectionAssert.AreEquivalent(items, result);
    }

    [TestMethod]
    public void GetRelatedContentItems_FiltersByAssociationType()
    {
        // Arrange
        var item1 = new ContentItem(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.All)
        {
            AssociationType = AssociationType.Individual
        };

        var item2 = new ContentItem(ContentType.Paragraph, companySize: CompanySize.All, sellerType: SellerType.All)
        {
            AssociationType = AssociationType.Subsidiary
        };

        var content = new Content { ContentItems = new List<ContentItem> { item1, item2 } };

        // Act
        var result = content.GetRelatedContentItems(AssociationType.Individual);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual(item1, result[0]);
    }

    [TestMethod]
    public void GetObligatedContentItems_ReturnsMatchingItems_AllCriteriaMatch()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.Large,
            SellerType = SellerType.All,
            RequiresNationData = true
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.Large, sellerType: SellerType.All)
        {
            RequiresNationData = true,
            AssociationType = AssociationType.Individual
        };
        var content = new Content { ContentItems = new List<ContentItem> { item } };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual(item, result[0]);
    }

    [TestMethod]
    public void GetObligatedContentItems_CompanySizeDoesNotMatch_ReturnsEmpty()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.Small,
            SellerType = SellerType.All,
            RequiresNationData = false
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.Large, sellerType: SellerType.All)
        {
            RequiresNationData = false,
            AssociationType = AssociationType.Individual
        };
        var content = new Content { ContentItems = new List<ContentItem> { item } };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void GetObligatedContentItems_SellerTypeDoesNotMatch_ReturnsEmpty()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.All,
            SellerType = SellerType.SellerOnly,
            RequiresNationData = false
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.NotSellerOnly)
        {
            RequiresNationData = false,
            AssociationType = AssociationType.Individual
        };
        var content = new Content { ContentItems = new List<ContentItem> { item } };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void GetObligatedContentItems_AssociationTypeIsNotSet_MatchesAny()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.All,
            SellerType = SellerType.All,
            RequiresNationData = false
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.All)
        {
            RequiresNationData = false,
            AssociationType = AssociationType.NotSet
        };
        var content = new Content { ContentItems = new List<ContentItem> { item } };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual(item, result[0]);
    }

    [TestMethod]
    public void GetObligatedContentItems_RequiresNationDataIsNull_MatchesAny()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.All,
            SellerType = SellerType.All,
            RequiresNationData = true
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.All)
        {
            AssociationType = AssociationType.Individual,
            RequiresNationData = null
        };
        var content = new Content { ContentItems = new List<ContentItem> { item } };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual(item, result[0]);
    }

    [TestMethod]
    public void GetObligatedContentItems_RequiresNationDataDoesNotMatch_ReturnsEmpty()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.All,
            SellerType = SellerType.All,
            RequiresNationData = true
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.All)
        {
            RequiresNationData = false,
            AssociationType = AssociationType.Individual
        };
        var content = new Content { ContentItems = new List<ContentItem> { item } };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void GetObligatedContentItems_AssociationTypeHasFlag_Matches()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.All,
            SellerType = SellerType.All,
            RequiresNationData = false
        };
        var item = new ContentItem(ContentType.Heading, companySize: CompanySize.All, sellerType: SellerType.All)
        {
            RequiresNationData = false,
            AssociationType = AssociationType.All
        };
        var content = new Content { ContentItems =[item] };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual(item, result[0]);
    }

    [TestMethod]
    public void GetObligatedContentItems_EmptyContentItems_ReturnsEmpty()
    {
        // Arrange
        var company = new CompanyModel
        {
            CompanySize = CompanySize.All,
            SellerType = SellerType.All,
            RequiresNationData = false
        };
        var content = new Content { ContentItems = new List<ContentItem>() };

        // Act
        var result = content.GetObligatedContentItems(company, AssociationType.Individual);

        // Assert
        Assert.IsEmpty(result);
    }
}