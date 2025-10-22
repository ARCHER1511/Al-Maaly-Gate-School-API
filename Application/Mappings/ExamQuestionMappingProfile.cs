using Application.DTOs.ExamDTOS;
using Application.DTOs.QuestionDTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class ExamQuestionMappingProfile : Profile
    {
        public ExamQuestionMappingProfile()
        {
            CreateMap<Exam, ExamViewDto>()
                .ForMember(dest => dest.QuestionIds,
                           opt => opt.MapFrom(src => src.QuestionExamTeachers.Select(qet => qet.QuestionId)));

            CreateMap<CreateExamDto, Exam>()
                .ForMember(dest => dest.QuestionExamTeachers, opt => opt.Ignore());

            CreateMap<Question, QuestionViewDto>().ReverseMap();
            CreateMap<CreateQuestionDto, Question>();
        }
    }
}
