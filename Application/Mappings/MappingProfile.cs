using Application.DTOs.AdminDTOs;
using Application.DTOs.AppointmentsDTOs;
using Application.DTOs.AuthDTOs;
using Application.DTOs.ClassAppointmentsDTOs;
using Application.DTOs.ClassDTOs;
using Application.DTOs.ExamDTOS;
using Application.DTOs.GradeDTOs;
using Application.DTOs.QuestionDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.StudentExamAnswerDTOs;
using Application.DTOs.SubjectDTOs;
using Application.DTOs.TeacherDTOs;
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
            // View mappings
            CreateMap<Teacher, TeacherViewDto>().IgnoreUnmapped();
            CreateMap<Teacher, TeacherAdminViewDto>().IgnoreUnmapped();

            // Reverse if needed
            CreateMap<TeacherAdminViewDto, Teacher>().IgnoreUnmapped();
            #endregion

            #region Student
            CreateMap<Student, StudentViewDto>().IgnoreUnmapped();
            CreateMap<StudentViewDto, Student>()
                .IgnoreUnmapped()
                .ForMember(
                    dest => dest.ClassId,
                    opt =>
                        opt.MapFrom(src =>
                            string.IsNullOrWhiteSpace(src.ClassId) ? null : src.ClassId
                        )
                );
            #endregion

            #region Classes Mappings
            CreateMap<Class, ClassDto>();
            CreateMap<ClassDto, Class>();

            // ADD THESE MAPPINGS:
            CreateMap<CreateClassDto, Class>();
            CreateMap<UpdateClassDto, Class>();

            CreateMap<Class, ClassViewDto>()
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.Grade.GradeName))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Students != null ? src.Students.Count : 0))
                .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.TeacherClasses != null ? src.TeacherClasses.Count : 0));
            #endregion

            #region Class Appointments
            CreateMap<ClassAppointment, ClassAppointmentDto>().IgnoreUnmapped();
            CreateMap<ClassAppointmentDto, ClassAppointment>()
                .IgnoreUnmapped()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ClassAppointment, StudentClassAppointmentDto>()
                .ForMember(
                    dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject.SubjectName)
                )
                .ForMember(
                    dest => dest.TeacherName,
                    opt => opt.MapFrom(src => src.Teacher.FullName)
                );
            #endregion

            #region Question Mappings

            CreateMap<ChoiceDto, Choices>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID generated automatically
                .ForMember(dest => dest.Question, opt => opt.Ignore())
                .IgnoreUnmapped();

            CreateMap<CreateQuestionDto, Question>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                //.ForMember(dest => dest.TextAnswer, opt => opt.MapFrom(src =>
                //    src.Type == QuestionTypes.Text && !string.IsNullOrWhiteSpace(src.TextAnswer)
                //        ? new TextAnswers { Content = src.TextAnswer }
                //        : null))
                //.ForMember(dest => dest.TrueAndFalses, opt => opt.MapFrom(src =>
                //    src.Type == QuestionTypes.TrueOrFalse && src.Choices != null
                //        ? new TrueAndFalses
                //        {
                //            IsTrue = src.Choices.Any(c => c.IsCorrect)
                //        }
                //        : null))
                .ForMember(
                    dest => dest.ChoiceAnswer,
                    opt =>
                        opt.MapFrom(src =>
                            src.Type == QuestionTypes.Choices
                            && !string.IsNullOrWhiteSpace(src.CorrectChoiceId)
                                ? new ChoiceAnswer { ChoiceId = src.CorrectChoiceId }
                                : null
                        )
                )
                .IgnoreUnmapped();

            CreateMap<Choices, ChoiceViewDto>().IgnoreUnmapped();

            CreateMap<Question, QuestionViewDto>()
                //.ForMember(dest => dest.TextAnswer,
                //    opt => opt.MapFrom(src => src.TextAnswer != null ? src.TextAnswer.Content : null))
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices))
                .ForMember(
                    dest => dest.CorrectChoiceId,
                    opt =>
                        opt.MapFrom(src =>
                            src.ChoiceAnswer != null ? src.ChoiceAnswer.ChoiceId : null
                        )
                )
                .IgnoreUnmapped();

            #endregion

            #region Exam Mappings
            // From Exam entity to detail view
            CreateMap<Exam, ExamDetailsViewDto>()
                .ForMember(
                    dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject.SubjectName)
                )
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName));

            // Basic bidirectional views
            CreateMap<Exam, ExamViewDto>()
                .ForMember(
                    dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject.SubjectName)
                )
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName));

            // Create mapping
            CreateMap<CreateExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID generated automatically
                .ForMember(dest => dest.Subject, opt => opt.Ignore()) // loaded later
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore()) // or .MapFrom(src => src.Questions) if you create them here
                .IgnoreUnmapped(); // your custom extension

            // Update mapping
            CreateMap<UpdateExamDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Class, opt => opt.Ignore())
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .IgnoreUnmapped();

            #endregion

            #region Subject Mappings
            CreateMap<Subject, SubjectViewDto>()
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.Grade.GradeName))
                .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.TeacherSubjects != null ? src.TeacherSubjects.Count : 0))
                .ForMember(dest => dest.ExamCount, opt => opt.MapFrom(src => src.Exams != null ? src.Exams.Count : 0));

            CreateMap<SubjectCreateDto, Subject>();
            CreateMap<SubjectUpdateDto, Subject>();
            #endregion

            #region StudentExamAnswer
            CreateMap<StudentExamAnswerDto, StudentExamAnswer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .IgnoreUnmapped();
            CreateMap<StudentExamAnswer, StudentExamAnswerDto>().IgnoreUnmapped();
            CreateMap<Exam, GetStudentExamsDto>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.ExamName))
                .ForMember(
                    dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject.SubjectName)
                )
                .ForMember(
                    dest => dest.TeacherName,
                    opt => opt.MapFrom(src => src.Teacher.FullName)
                )
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.Teacher.Id));

            CreateMap<Exam, ExamQuestionsDto>()
                .ForMember(dest => dest.ExamId, opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject.SubjectName)
                )
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

            CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices));

            CreateMap<Choices, ChoicesDto>();
            #endregion

            #region Student Exam Result
            CreateMap<StudentExamResult, StudentExamResultDto>().IgnoreUnmapped();
            CreateMap<StudentExamResultDto, StudentExamResult>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .IgnoreUnmapped();
            #endregion

            #region Grade
            CreateMap<Grade, GradeViewDto>()
                .ForMember(dest => dest.ClassCount, opt => opt.MapFrom(src => src.Classes.Count))
                .ForMember(dest => dest.SubjectCount, opt => opt.MapFrom(src => src.Subjects.Count));

            CreateMap<CreateGradeDto, Grade>();
            CreateMap<UpdateGradeDto, Grade>();

            CreateMap<Grade, GradeWithDetailsDto>();

            // Class mappings
            CreateMap<Class, ClassViewDto>()
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.Grade.GradeName))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Students != null ? src.Students.Count : 0))
                .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.TeacherClasses != null ? src.TeacherClasses.Count : 0));

            CreateMap<ClassDto, Class>();
            CreateMap<CreateClassInGradeDto, Class>();

            // Subject mappings
            CreateMap<Subject, SubjectViewDto>()
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.Grade.GradeName))
                .ForMember(dest => dest.TeacherCount, opt => opt.MapFrom(src => src.TeacherSubjects != null ? src.TeacherSubjects.Count : 0))
                .ForMember(dest => dest.ExamCount, opt => opt.MapFrom(src => src.Exams != null ? src.Exams.Count : 0));

            CreateMap<SubjectCreateDto, Subject>();
            CreateMap<SubjectUpdateDto, Subject>();
            #endregion
        }
    }
}
