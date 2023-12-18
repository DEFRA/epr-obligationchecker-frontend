namespace FrontendObligationChecker.Models.ObligationChecker;

public static class PagePath
{
    public const string TypeOfOrganisation = "type-of-organisation";
    public const string AnnualTurnover = "annual-turnover";
    public const string OwnBrand = "supplying-goods-under-own-brand";
    public const string UnbrandedPackaging = "placing-goods-into-unbranded-packaging";
    public const string ImportingProducts = "importing-products-in-packaging";
    public const string SupplyingEmptyPackaging = "supplying-empty-packaging";
    public const string HiringLoaning = "hiring-loaning-reusable-packaging";
    public const string OnlineMarketplace = "owning-online-marketplace";
    public const string SupplyingFilledPackaging = "supplying-filled-packaging-customers";
    public const string NoActionNeeded = "no-action-needed";
    public const string AmountYouSupply = "amount-you-supply";
    public const string WhatYouNeedToDo = "what-you-need-to-do";
    public const string PlaceDrinksOnMarket = "place-drinks-on-market";
    public const string MaterialsForDrinksContainers = "materials-for-drinks-containers";
    public const string ContainerVolume = "container-volume";

    public static bool IsActivityPagePath(string path)
    {
        return path is PagePath.OwnBrand
                    or PagePath.UnbrandedPackaging
                    or PagePath.ImportingProducts
                    or PagePath.OnlineMarketplace
                    or PagePath.SupplyingEmptyPackaging
                    or PagePath.HiringLoaning
                    or PagePath.SupplyingFilledPackaging;
    }
}