namespace FrontendObligationChecker.Models.ObligationChecker;

public static class QuestionKey
{
    public const string DemoOne = "demo-one";
    public const string DemoTwo = "demo-two";
    public const string DemoThree = "demo-three";
    public const string TypeOfOrganisation = "type-of-organisation";
    public const string AnnualTurnover = "annual-turnover";
    public const string OwnBrand = "supplying-goods-under-own-brand";
    public const string UnbrandedPackaging = "placing-goods-into-unbranded-packaging";
    public const string ImportingProducts = "importing-products-in-packaging";
    public const string SellingEmptyPackaging = "selling-empty-packaging";
    public const string HiringLoaning = "hiring-loaning-reusable-packaging";
    public const string OnlineMarketplace = "owning-online-marketplace";
    public const string SupplyingFilledPackaging = "supplying-filled-packaging-consumers";
    public const string AmountYouSupply = "amount-you-handle";
    public const string MaterialsForDrinksContainers = "materials-for-drinks-containers";
    public const string SingleUseContainersOnMarket = "single-use-containers-on-market";
    public const string ContainerVolume = "container-volume";

    public static string GetYesNoPageQuestionKey(string pagePath)
    {
        return pagePath switch
        {
            PagePath.DemoOne => DemoOne,
            PagePath.DemoTwo => DemoTwo,
            PagePath.DemoThree => DemoThree,
            PagePath.PlaceDrinksOnMarket => SingleUseContainersOnMarket,
            PagePath.ContainerVolume => ContainerVolume,
            PagePath.OwnBrand => OwnBrand,
            PagePath.UnbrandedPackaging => UnbrandedPackaging,
            PagePath.ImportingProducts => ImportingProducts,
            PagePath.SupplyingEmptyPackaging => SellingEmptyPackaging,
            PagePath.HiringLoaning => HiringLoaning,
            PagePath.OnlineMarketplace => OnlineMarketplace,
            PagePath.SupplyingFilledPackaging => SupplyingFilledPackaging,
            _ => throw new ArgumentException("PagePath must be of single yes-no question page")
        };
    }
}