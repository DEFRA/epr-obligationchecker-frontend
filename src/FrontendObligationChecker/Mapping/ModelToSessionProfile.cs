namespace FrontendObligationChecker.Mapping;

using AutoMapper;
using Models.ObligationChecker;
using Models.Session;

public class ModelToSessionProfile : Profile
{
    public ModelToSessionProfile()
    {
        CreateMap<Page, SessionPage>();
        CreateMap<Question, SessionQuestion>();
    }
}