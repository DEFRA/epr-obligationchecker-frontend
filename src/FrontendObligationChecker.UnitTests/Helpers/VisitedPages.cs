using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.ViewModels;

namespace FrontendObligationChecker.UnitTests.Helpers;

public enum TypeOfOrganisation
{
    NotSet = 0,
    ParentCompany,
    Subsidiary,
    IndividualCompany
}

public enum AnnualTurnover
{
    NotSet = 0,
    UnderOneMillion,
    OneMillionToTwoMillion,
    OverTwoMillion
}

public enum YesNo
{
    NotSet = 0,
    Yes = 1,
    No = 2
}

[Flags]
public enum MaterialsForDrinksContainers
{
    NotSet = 0,
    None = 1 << 0,
    PlasticBottles = 1 << 1,
    GlassBottles = 1 << 2,
    SteelCans = 1 << 3,
    AluminiumCans = 1 << 4
}

public enum AmountYouSupply
{
    NotSet = 0,
    HandleUnder25Tonnes,
    Handle25To50Tonnes,
    Handle50TonnesOrMore
}

public record VisitedPages
{
    public TypeOfOrganisation TypeOfOrganisation { get; set; }

    public AnnualTurnover AnnualTurnover { get; set; }

    public YesNo OwnBrand { get; set; }

    public YesNo UnbrandedPackaging { get; set; }

    public YesNo ImportingProducts { get; set; }

    public YesNo SellingEmptyPackaging { get; set; }

    public YesNo HiringLoaning { get; set; }

    public YesNo OnlineMarketplace { get; set; }

    public YesNo SupplyingFilledPackaging { get; set; }

    public YesNo PlaceDrinksOnMarket { get; set; }

    public MaterialsForDrinksContainers MaterialsForDrinksContainers { get; set; }

    public YesNo ContainerVolume { get; set; }

    public AmountYouSupply AmountYouSupply { get; set; }
}