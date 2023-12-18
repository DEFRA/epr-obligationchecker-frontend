using AutoMapper;
using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;
using FrontendObligationChecker.Services.Session.Interfaces;

namespace FrontendObligationChecker.Services.Session;
public class JourneySession : IJourneySession
{
    private readonly IDistributedSession<SessionJourney> _distributedSession;
    private readonly IMapper _mapper;
    private readonly ILogger<JourneySession> _logger;

    public JourneySession(
        IDistributedSession<SessionJourney> distributedSession
        , IMapper mapper
        , ILogger<JourneySession> logger)
    {
        _distributedSession = distributedSession;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task AddPageAsync(Page page)
    {
        try
        {
            _logger.LogEnter();
            var sessionPage = _mapper.Map<SessionPage>(page);

            var sessionJourney = await GetAsync() ?? new SessionJourney();
            sessionJourney.RemovePage(page.Path);
            sessionJourney.Pages.Add(sessionPage);

            RemoveSession();

            await _distributedSession.SetAsync(nameof(SessionJourney), sessionJourney);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding page to session");
        }
        finally
        {
            _logger.LogExit();
        }
    }

    public async Task<SessionJourney?> GetAsync()
    {
        return await _distributedSession.GetAsync(nameof(SessionJourney));
    }

    public async Task RemovePagesAfterCurrentAsync(Page page)
    {
        try
        {
            _logger.LogEnter();
            var sessionJourney = await GetAsync();
            sessionJourney.Pages = sessionJourney.Pages.Where(x => x.Index <= page.Index).ToList();

            RemoveSession();

            await _distributedSession.SetAsync(nameof(SessionJourney), sessionJourney);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding page to session");
        }
        finally
        {
            _logger.LogExit();
        }
    }

    public void RemoveSession()
    {
        _distributedSession.Remove(nameof(SessionJourney));
    }

    public bool IsSessionValid(string path)
    {
        throw new NotImplementedException();
    }
}