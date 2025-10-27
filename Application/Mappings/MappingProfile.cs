using Application.DTOs.AdminDTOs;
using Application.DTOs.AppointmentsDTOs;
using Application.DTOs.AuthDTOs;
using Application.DTOs.QuestionDTOs;
using Application.DTOs.ClassDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.TeacherDTOs;
using Application.Services;
using AutoMapper;
using Common.Extensions;
using Domain.Entities;

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
            CreateMap<AdminCreateDto, Admin>().IgnoreUnmapped();
            //Edit
            CreateMap<AdminUpdateDto, Admin>().IgnoreUnmapped().ReverseMap();
            //View
            CreateMap<Admin, AdminViewDto>();
            #endregion

            #region Auth Mappings
            CreateMap<RegisterRequest, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .IgnoreUnmapped();

            CreateMap<AppUser, AuthResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .IgnoreUnmapped();

            // ===== ADMIN =====
            CreateMap<RegisterRequest, Admin>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(
                    dest => dest.AppUser,
                    opt =>
                        opt.MapFrom(src => new AppUser { UserName = src.Email, Email = src.Email })
                )
                .IgnoreUnmapped();

            // ===== TEACHER =====
            CreateMap<RegisterRequest, Teacher>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(
                    dest => dest.AppUser,
                    opt =>
                        opt.MapFrom(src => new AppUser { UserName = src.Email, Email = src.Email })
                )
                .IgnoreUnmapped();

            // ===== STUDENT =====
            CreateMap<RegisterRequest, Student>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(
                    dest => dest.AppUser,
                    opt =>
                        opt.MapFrom(src => new AppUser { UserName = src.Email, Email = src.Email })
                )
                .IgnoreUnmapped();

            // ===== PARENT =====
            CreateMap<RegisterRequest, Parent>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(
                    dest => dest.AppUser,
                    opt =>
                        opt.MapFrom(src => new AppUser { UserName = src.Email, Email = src.Email })
                )
                .IgnoreUnmapped();
            #endregion

            #region Teacher Mappings
            //View
            CreateMap<Teacher, TeacherViewDto>();
            #endregion

            #region Teacher Mappings
            CreateMap<Student, StudentViewDto>().IgnoreUnmapped();
            #endregion

            #region Classes Mappings
            CreateMap<Class, ClassViewDto>().IgnoreUnmapped();

            CreateMap<ClassDto, Class>()
            .ForMember(dest => dest.Students, opt => opt.Ignore())
            .ForMember(dest => dest.ClassAssets, opt => opt.Ignore());

            CreateMap<Class,ClassDto>();

            CreateMap<ClassAppointment, ClassAppointmentsDTo>().IgnoreUnmapped();
            #endregion

            #region Appointments
            CreateMap<ClassAppointment, AppointmentDto>().IgnoreUnmapped();
            CreateMap<ClassAppointment, ViewAppointmentDto>().IgnoreUnmapped();
            CreateMap<AppointmentDto, ClassAppointment>().IgnoreUnmapped()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
            #endregion

            #region Question Mappings

            //View
            CreateMap<Question, QuestionViewDto>();
            #endregion
        }
    }
}
