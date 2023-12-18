namespace FrontendObligationChecker.Models.Session;

public class SessionJourney
{
    public List<SessionPage> Pages { get; set; } = new();

    public void RemovePage(string path)
    {
        var existingPage = Pages.SingleOrDefault(x => x.Path == path);
        if (existingPage != null)
        {
            Pages.Remove(existingPage);
        }
    }
}