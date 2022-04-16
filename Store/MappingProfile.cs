using AutoMapper;
using Store.BLL;
using Store.BLL.Entities;
using Store.Models;

namespace Store;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserViewModel, User>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password.GetMD5Hash()));
        CreateMap<User, UserViewModel>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => string.Empty));
    }
}
