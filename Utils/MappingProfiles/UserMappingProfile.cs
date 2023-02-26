using AutoMapper;
using Entities.Database;
using Entities.Domain;
using Entities.Models;

namespace BigBrother.Helpers.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserModel, User>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid()));

        CreateMap<UserModel, User>().ReverseMap();

        CreateMap<User, UserEntity>().ReverseMap();
    }
}
