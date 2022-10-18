using AutoMapper;
using DataProcessingContext;

namespace DataProcessingCore
{
    public class DomainToDtoProfile : Profile
    {

        public DomainToDtoProfile()
        {
            CreateMap<AppUser, UserOutputDto>();
            CreateMap<UserSignUpInputDto, AppUser>();
        }

    }
}
