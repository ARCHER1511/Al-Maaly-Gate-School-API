﻿using Application.DTOs.AdminDTOs;
using Application.DTOs.AppointmentsDTOs;
using Application.DTOs.AuthDTOs;
using Application.DTOs.ClassAppointmentDTOs;
using Application.DTOs.ClassDTOs;
using Application.DTOs.ExamDTOS;
using Application.DTOs.QuestionDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.SubjectDTOs;
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
            CreateMap<Admin, AdminViewDto>().IgnoreUnmapped();
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
            CreateMap<Teacher, TeacherViewDto>().IgnoreUnmapped();
            #endregion

            #region Teacher Mappings
            CreateMap<Student, StudentViewDto>().IgnoreUnmapped();
            #endregion

            #region Classes Mappings
            CreateMap<Class, ClassViewDto>().IgnoreUnmapped();

            CreateMap<ClassDto, Class>().IgnoreUnmapped();

            CreateMap<Class,ClassDto>().IgnoreUnmapped();

            CreateMap<ClassAppointment, ClassAppointmentsDTo>().IgnoreUnmapped();
            #endregion

            #region Appointments
            CreateMap<ClassAppointment, AppointmentDto>().IgnoreUnmapped();
            CreateMap<ClassAppointment, ViewAppointmentDto>().IgnoreUnmapped();
            CreateMap<AppointmentDto, ClassAppointment>().IgnoreUnmapped()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateClassAppointmentDto, ClassAppointment>();
            CreateMap<ClassAppointment, ClassAppointmentViewDto>();
            #endregion

            #region Question Mappings

            CreateMap<ChoiceDto, Choices>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID generated automatically
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .IgnoreUnmapped();

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                .ForMember(dest => dest.TextAnswer, opt => opt.MapFrom(src =>
                    src.Type == QuestionTypes.Text && !string.IsNullOrWhiteSpace(src.TextAnswer)
                        ? new TextAnswers { Content = src.TextAnswer }
                        : null))
                .ForMember(dest => dest.TrueAndFalses, opt => opt.MapFrom(src =>
                    src.Type == QuestionTypes.TrueOrFalse && src.Choices != null
                        ? new TrueAndFalses
                        {
                            IsTrue = src.Choices.Any(c => c.IsCorrect)
                        }
                        : null))
                .ForMember(dest => dest.ChoiceAnswer, opt => opt.MapFrom(src =>
                    src.Type == QuestionTypes.Choices && !string.IsNullOrWhiteSpace(src.CorrectChoiceId)
                        ? new ChoiceAnswer { ChoiceId = src.CorrectChoiceId }
                        : null))
                .IgnoreUnmapped();

            CreateMap<Choices, ChoiceViewDto>().IgnoreUnmapped();

            CreateMap<Question, QuestionViewDto>()
                .ForMember(dest => dest.TextAnswer,
                    opt => opt.MapFrom(src => src.TextAnswer != null ? src.TextAnswer.Content : null))
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                .ForMember(dest => dest.CorrectChoiceId,
                    opt => opt.MapFrom(src => src.ChoiceAnswer != null ? src.ChoiceAnswer.ChoiceId : null))
                .IgnoreUnmapped();

            #endregion

            #region Exam Mappings
            CreateMap<Exam, ExamDetailsViewDto>()
    .               ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

            CreateMap<Exam, ExamViewDto>().ReverseMap();
            CreateMap<CreateExamDto, Exam>().ReverseMap();
            CreateMap<UpdateExamDto, Exam>().ReverseMap();
            //View
            CreateMap<Question, QuestionViewDto>().IgnoreUnmapped();
            #endregion

            #region Subject Mappings
            //Create
            CreateMap<SubjectCreateDto, Subject>().IgnoreUnmapped();
            //Edit
            CreateMap<SubjectUpdateDto, Subject>().IgnoreUnmapped().ReverseMap();
            //View
            CreateMap<Subject, SubjectViewDto>().IgnoreUnmapped();
            #endregion
        }
    }
}
