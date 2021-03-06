using AutoMapper;
using MongoDB.Bson;
using Store.BLL;
using Store.BLL.Entities;
using Store.DLL.Entities;
using Store.Models;

namespace Store;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserViewModel, User>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password.GetMD5Hash()));
        CreateMap<User, UserViewModel>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => string.Empty));

        CreateMap<ProductMongo, Product>().ForMember(dest => dest.Id, opt => opt
        .MapFrom(src => src.Id.ToString()));

        CreateMap<Product, ProductMongo>().ForMember(dest => dest.Id, opt => opt
        .MapFrom(src => string.IsNullOrEmpty(src.Id) ? MongoDB.Bson.ObjectId.GenerateNewId() : MongoDB.Bson.ObjectId.Parse(src.Id)));

        CreateMap<Product, ProductViewModel>().ReverseMap();
    }
}
