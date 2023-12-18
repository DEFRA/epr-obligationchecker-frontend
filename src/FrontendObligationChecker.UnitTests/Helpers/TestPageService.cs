using AutoMapper;

using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.Services.Session;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace FrontendObligationChecker.UnitTests.Helpers;

public static class TestPageService
{
    public static PageService GetPageService()
    {
        var distributedSession = new TestDistributedSession<SessionJourney>();

        var config = new MapperConfiguration(o =>
        {
            o.CreateMap<Page, SessionPage>();
            o.CreateMap<Question, SessionQuestion>();
        });
        var mapper = config.CreateMapper();

        var logger = new Mock<ILogger<JourneySession>>();

        var journeySession = new JourneySession(distributedSession, mapper, logger.Object);

        var externalUrls = new Mock<IOptions<ExternalUrlsOptions>>();

        return new PageService(journeySession, externalUrls.Object);
    }
}