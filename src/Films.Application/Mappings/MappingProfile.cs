using AutoMapper;
using Films.Application.DTOs;
using Films.Domain.Entities;

namespace Films.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Film mappings
        CreateMap<Film, FilmDto>();
        CreateMap<FilmCreateDto, Film>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"));
        CreateMap<FilmUpdateDto, Film>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"));

        // Actor mappings
        CreateMap<Actor, ActorDto>()
            .ForMember(dest => dest.SexName, opt => opt.MapFrom(src => src.Sex != null ? src.Sex.Name : null));
        CreateMap<ActorCreateDto, Actor>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"));
        CreateMap<ActorUpdateDto, Actor>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"));

        // DirectedBy mappings
        CreateMap<DirectedBy, DirectedByDto>()
            .ForMember(dest => dest.SexName, opt => opt.MapFrom(src => src.Sex != null ? src.Sex.Name : null));
        CreateMap<DirectedByCreateDto, DirectedBy>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"));
        CreateMap<DirectedByUpdateDto, DirectedBy>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"));

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.TypeUserName, opt => opt.MapFrom(src => src.TypeUser != null ? src.TypeUser.Name : null));
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"));
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"));

        // Sex mappings
        CreateMap<Sex, SexDto>();
        CreateMap<SexCreateDto, Sex>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"));
        CreateMap<SexUpdateDto, Sex>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"));

        // TypeUser mappings
        CreateMap<TypeUser, TypeUserDto>();
        CreateMap<TypeUserCreateDto, TypeUser>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"));
        CreateMap<TypeUserUpdateDto, TypeUser>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"));
    }
}
