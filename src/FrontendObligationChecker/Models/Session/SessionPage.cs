namespace FrontendObligationChecker.Models.Session;

public class SessionPage
{
    public string Path { get; set; } = default!;

    public int Index { get; set; }

    public List<SessionQuestion> Questions { get; set; } = new();
}