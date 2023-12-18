using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;

namespace FrontendObligationChecker.Services.Session.Interfaces;
public interface IJourneySession
{
    Task AddPageAsync(Page page);

    Task<SessionJourney?> GetAsync();

    Task RemovePagesAfterCurrentAsync(Page page);

    void RemoveSession();

    bool IsSessionValid(string path);
}