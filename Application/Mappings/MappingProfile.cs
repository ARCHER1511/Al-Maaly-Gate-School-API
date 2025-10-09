using AutoMapper;
using Domain.Entities;
using Application.DTOs.AdminDTOs;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            #region Mapping Template
            //for create mappings
            //CreateMap<TDTO,T>();
            //for edit mappings
            //CreateMap<T,TDTO>().ReverseMap();
            //for view T mappings
            #endregion


            #region Admin Mappings
            //Create
            CreateMap<AdminCreateDto, Admin>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AppUser, opt => opt.Ignore());
            //Edit
            CreateMap<AdminUpdateDto, Admin>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AppUser, opt => opt.Ignore())
                .ReverseMap();
            //View
            CreateMap<Admin, AdminViewDto>();
            #endregion
        }
    }
}
