using AutoMapper;
using DataProcessingContext;

namespace DataProcessingAPI
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
