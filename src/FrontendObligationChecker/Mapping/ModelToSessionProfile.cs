using AutoMapper;
using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Models.Session;

namespace FrontendObligationChecker.Mapping;
public class ModelToSessionProfile : Profile
{
    public ModelToSessionProfile()
    {
        CreateMap<Page, SessionPage>();
        CreateMap<Question, SessionQuestion>();
    }
}