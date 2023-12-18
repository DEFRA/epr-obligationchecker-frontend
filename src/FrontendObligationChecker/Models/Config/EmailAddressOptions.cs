namespace FrontendObligationChecker.Models.Config;

public class EmailAddressOptions
{
    public const string ConfigSection = "EmailAddresses";

    public string DataProtection { get; set; }

    public string DefraGroupProtectionOfficer { get; set; }

    public string InformationCommissioner { get; set; }
}