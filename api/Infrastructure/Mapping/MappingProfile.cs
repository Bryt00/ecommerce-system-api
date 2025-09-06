using AutoMapper;
using ecommapi.Application.Contracts;
using ecommapi.Domain.Models;

namespace ecommapi.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<User, CurrentUserResponse>();
         }
    }
}
