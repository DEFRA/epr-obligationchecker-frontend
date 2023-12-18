using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;

namespace FrontendObligationChecker.UnitTests.Helpers;
public static class SessionHelper
{
    public static SessionPage GetOrganisationTypeSessionPage()
    {
        return new SessionPage()
        {
            Index = 10,
            Path = PagePath.TypeOfOrganisation,
            Questions = new List<SessionQuestion>
            {
                new()
                {
                    Key = QuestionKey.TypeOfOrganisation,
                    Answer = "parent"
                }
            }
        };
    }

    public static SessionJourney GetSessionJourney()
    {
        return new SessionJourney()
        {
            Pages = new List<SessionPage>
            {
                GetOrganisationTypeSessionPage(),
                GetTurnoverSessionPage()
            }
        };
    }

    private static SessionPage GetTurnoverSessionPage()
    {
        return new SessionPage()
        {
            Index = 20,
            Path = PagePath.AnnualTurnover,
            Questions = new List<SessionQuestion>
            {
                new()
                {
                    Key = QuestionKey.AnnualTurnover,
                    Answer = "1"
                }
            }
        };
    }
}