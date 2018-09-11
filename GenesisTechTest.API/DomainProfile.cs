using AutoMapper;
using GenesisTechTest.API.Models;
using GenesisTechTest.Common.Models;

namespace GenesisTechTest.API
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<SignUpRequest, User>();
            CreateMap<User, SignUpResponse>();
            CreateMap<User, SignInResponse>();
        }
    }
}
