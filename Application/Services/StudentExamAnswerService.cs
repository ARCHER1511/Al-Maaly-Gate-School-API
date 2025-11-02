using Application.DTOs.StudentExamAnswerDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Application.Services
{
    public class StudentExamAnswerService : IStudentExamAnswerService
    {
        private readonly IStudentExamAnswerRepository _studentExamAnswerRepository;
        private readonly IExamRepository _ExamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentExamAnswerService(IStudentExamAnswerRepository studentExamAnswerRepository, IExamRepository examRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentExamAnswerRepository = studentExamAnswerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ExamRepository = examRepository;
        }

        public async Task<ServiceResult<IEnumerable<StudentExamAnswerDto>>> GetAllAsync()
        {
            var students = await _studentExamAnswerRepository.GetAllAsync();
            if (students == null) return ServiceResult<IEnumerable<StudentExamAnswerDto>>.Fail("Students Answers not found");

            var studentsDto = _mapper.Map<IEnumerable<StudentExamAnswerDto>>(students);
            return ServiceResult<IEnumerable<StudentExamAnswerDto>>.Ok(studentsDto, "Students Answers retrieved successfully");
        }
        public async Task<ServiceResult<StudentExamAnswerDto>> GetByIdAsync(object id)
        {
            var student = await _studentExamAnswerRepository.GetByIdAsync(id);
            if (student == null) return ServiceResult<StudentExamAnswerDto>.Fail("Student Answer not found");

            var studentDto = _mapper.Map<StudentExamAnswerDto>(student);
            return ServiceResult<StudentExamAnswerDto>.Ok(studentDto, "Student Answer retrieved successfully");
        }
        public async Task<ServiceResult<StudentExamAnswerDto>> CreateAsync(StudentExamAnswerDto dto)
        {
            var student = _mapper.Map<StudentExamAnswer>(dto);

            //var question =  _ExamRepository.AsQueryable()
            //    .Where(e => e.Id == dto.ExamId).Include(m => m.Questions)
            //    .Where(q => q.Id == dto.QuestionId);

            //if (question.Type == 2 || question.Type == 3) {
            //    Question.
            //}

                await _studentExamAnswerRepository.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamAnswerDto>(student);
            return ServiceResult<StudentExamAnswerDto>.Ok(viewDto, "Student Answer created successfully");
        }
        public async Task<ServiceResult<StudentExamAnswerDto>> UpdateAsync(StudentExamAnswerDto dto)
        {
            var existingStudent = await _studentExamAnswerRepository.GetByIdAsync(dto.Id);
            if (existingStudent == null)
                return ServiceResult<StudentExamAnswerDto>.Fail("Student Answer not found");

            _mapper.Map(dto, existingStudent);

            _studentExamAnswerRepository.Update(existingStudent);
            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamAnswerDto>(existingStudent);
            return ServiceResult<StudentExamAnswerDto>.Ok(viewDto, "Student Answer updated successfully");
        }
        public async Task<ServiceResult<bool>> DeleteAsync(object id)
        {
            var student = await _studentExamAnswerRepository.GetByIdAsync(id);
            if (student == null)
                return ServiceResult<bool>.Fail("Student Answer not found");

            _studentExamAnswerRepository.Delete(student);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Student Answer deleted successfully");
        }

        //public async Task<ServiceResult<IEnumerable<GetStudentExamsDto>>> GetExams(string ClassId)
        //{
        //    var exams = await _subjectRepository.AsQueryable()
        //                .Where(s => s.ClassId == ClassId)
        //                .Include(s => s.Exams)
        //                .ToListAsync();

        //    if (exams == null) return ServiceResult<IEnumerable<GetStudentExamsDto>>.Fail("Couldn't find any Exam");

        //    var ExamsDto = _mapper.Map<IEnumerable<GetStudentExamsDto>>(exams);

        //    return ServiceResult<IEnumerable<GetStudentExamsDto>>.Ok(ExamsDto, "All Exams Retrived successfully");
        //}

    }
}
