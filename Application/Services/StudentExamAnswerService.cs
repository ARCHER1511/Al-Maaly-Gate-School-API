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
        private readonly IStudentExamResultRepository _studentExamResultRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IExamRepository _ExamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentExamAnswerService(IStudentExamAnswerRepository studentExamAnswerRepository, IStudentExamResultRepository studentExamResultRepository, ISubjectRepository subjectRepository, IExamRepository examRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentExamAnswerRepository = studentExamAnswerRepository;
            _studentExamResultRepository = studentExamResultRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ExamRepository = examRepository;
            _subjectRepository = subjectRepository;
        }

        public async Task<ServiceResult<IEnumerable<StudentExamAnswerDto>>> GetExamsTextQuestions(
            string examId, string subjectId, string classId)
        {
            var textAnswers = await _studentExamAnswerRepository.AsQueryable()
                .Where(a =>
                    a.ExamId == examId &&
                    a.Exam.SubjectId == subjectId &&
                    a.Exam.ClassId == classId &&
                    a.Question.Type == QuestionTypes.Text)
                .Include(a => a.Student)
                .Include(a => a.Question)
                .Include(a => a.Exam)
                .ToListAsync();

            if (!textAnswers.Any())
                return ServiceResult<IEnumerable<StudentExamAnswerDto>>.Fail("No text questions found for this exam");

            var dto = _mapper.Map<IEnumerable<StudentExamAnswerDto>>(textAnswers);
            return ServiceResult<IEnumerable<StudentExamAnswerDto>>.Ok(dto, "Text questions retrieved successfully");
        }

        public async Task<ServiceResult<StudentExamAnswerDto>> UpdateStudentTextAnswerMark(StudentExamAnswerDto dto)
        {
            var studentAnswer = await _studentExamAnswerRepository.AsQueryable()
                .FirstOrDefaultAsync(a =>
                    a.Id == dto.Id &&
                    a.ExamId == dto.ExamId &&
                    a.StudentId == dto.StudentId);

            if (studentAnswer == null)
                return ServiceResult<StudentExamAnswerDto>.Fail("Student answer not found");

            studentAnswer.Mark = dto.Mark;
            _studentExamAnswerRepository.Update(studentAnswer);

            await _unitOfWork.SaveChangesAsync();

            var exam = await _ExamRepository.FirstOrDefaultAsync(e => e.Id == dto.ExamId);
            if (exam == null)
                return ServiceResult<StudentExamAnswerDto>.Fail("Exam not found");

            var allAnswers = await _studentExamAnswerRepository.AsQueryable()
                .Where(a => a.StudentId == dto.StudentId && a.ExamId == dto.ExamId)
                .ToListAsync();

            decimal totalMark = allAnswers.Sum(a => a.Mark ?? 0);
            decimal percentage = exam.FullMark == 0
                ? 0
                : Math.Round((totalMark / (decimal)exam.FullMark) * 100, 2);

            string status = totalMark >= exam.MinMark ? "Success" : "Fail";

            var existingResult = await _studentExamResultRepository.AsQueryable()
                .FirstOrDefaultAsync(r => r.StudentId == dto.StudentId && r.ExamId == dto.ExamId);

            if (existingResult != null)
            {
                existingResult.TotalMark = Math.Round(totalMark, 2);
                existingResult.Percentage = percentage;
                existingResult.Status = status;
                existingResult.FullMark = exam.FullMark;
                existingResult.MinMark = exam.MinMark;

                _studentExamResultRepository.Update(existingResult);
            }
            else
            {
                var result = new StudentExamResult
                {
                    StudentId = dto.StudentId,
                    ExamId = dto.ExamId,
                    TotalMark = Math.Round(totalMark, 2),
                    FullMark = exam.FullMark,
                    MinMark = exam.MinMark,
                    Percentage = percentage,
                    Status = status
                };

                await _studentExamResultRepository.AddAsync(result);
            }

            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamAnswerDto>(studentAnswer);
            return ServiceResult<StudentExamAnswerDto>.Ok(viewDto, "Mark updated and exam result recalculated successfully.");
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
            var question = await _ExamRepository.AsQueryable()
                .Where(e => e.Id == dto.ExamId)
                .SelectMany(e => e.Questions)
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == dto.QuestionId);

            if (question == null)
                return ServiceResult<StudentExamAnswerDto>.Fail("You are trying to answer a question that does not exist.");

            var existingAnswer = await _studentExamAnswerRepository.AsQueryable()
                .FirstOrDefaultAsync(a =>
                    a.StudentId == dto.StudentId &&
                    a.ExamId == dto.ExamId &&
                    a.QuestionId == dto.QuestionId);

            StudentExamAnswer studentAnswer;

            if (existingAnswer != null)
            {
                studentAnswer = existingAnswer;
                studentAnswer.Mark = 0;

                switch (question.Type)
                {
                    case QuestionTypes.Choices:
                        var correctChoice = question.Choices?.FirstOrDefault(c => c.IsCorrect);
                        studentAnswer.ChoiceId = dto.ChoiceId;
                        studentAnswer.TextAnswer = null;
                        studentAnswer.TrueAndFalseAnswer = null;

                        if (correctChoice != null && correctChoice.Id == dto.ChoiceId)
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        break;

                    case QuestionTypes.TrueOrFalse:
                        studentAnswer.TrueAndFalseAnswer = dto.TrueAndFalseAnswer;
                        studentAnswer.TextAnswer = null;
                        studentAnswer.ChoiceId = null;

                        if (question.TrueAndFalses == dto.TrueAndFalseAnswer)
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        break;

                    case QuestionTypes.Text:
                        studentAnswer.TextAnswer = dto.TextAnswer;
                        studentAnswer.ChoiceId = null;
                        studentAnswer.TrueAndFalseAnswer = null;
                        studentAnswer.Mark = 0;
                        break;
                }

                _studentExamAnswerRepository.Update(studentAnswer);
            }
            else
            {
                studentAnswer = _mapper.Map<StudentExamAnswer>(dto);
                studentAnswer.Mark = 0;

                switch (question.Type)
                {
                    case QuestionTypes.Choices:
                        var correctChoice = question.Choices?.FirstOrDefault(c => c.IsCorrect);
                        if (correctChoice != null && correctChoice.Id == dto.ChoiceId)
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        break;

                    case QuestionTypes.TrueOrFalse:
                        if (question.TrueAndFalses == dto.TrueAndFalseAnswer)
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        break;
                }

                await _studentExamAnswerRepository.AddAsync(studentAnswer);
            }

            var exam = await _ExamRepository.FirstOrDefaultAsync(e => e.Id == dto.ExamId);
            var allAnswers = await _studentExamAnswerRepository.AsQueryable()
                .Where(a => a.StudentId == dto.StudentId && a.ExamId == dto.ExamId)
                .ToListAsync();

            decimal totalMark = allAnswers.Sum(a => a.Mark ?? 0);
            decimal percentage = exam.FullMark == 0 ? 0 : Math.Round((totalMark / (decimal)exam.FullMark) * 100, 2);
            string status = totalMark >= exam.MinMark ? "Success" : "Fail";

            var existingResult = await _studentExamResultRepository.AsQueryable()
                .FirstOrDefaultAsync(r => r.StudentId == dto.StudentId && r.ExamId == dto.ExamId);

            if (existingResult != null)
            {
                existingResult.TotalMark = Math.Round(totalMark, 2);
                existingResult.Percentage = percentage;
                existingResult.Status = status;
                existingResult.FullMark = exam.FullMark;
                existingResult.MinMark = exam.MinMark;
                _studentExamResultRepository.Update(existingResult);
            }
            else
            {
                var result = new StudentExamResult
                {
                    StudentId = dto.StudentId,
                    ExamId = dto.ExamId,
                    TotalMark = Math.Round(totalMark, 2),
                    FullMark = exam.FullMark,
                    MinMark = exam.MinMark,
                    Percentage = percentage,
                    Status = status
                };

                await _studentExamResultRepository.AddAsync(result);
            }

            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamAnswerDto>(studentAnswer);
            return ServiceResult<StudentExamAnswerDto>.Ok(viewDto, "Student answer saved and result updated successfully.");
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

        public async Task<ServiceResult<IEnumerable<GetStudentExamsDto>>> GetExams(string classId)
        {
            var exams = await _subjectRepository.AsQueryable()
                .Where(s => s.ClassId == classId)
                .Include(s => s.Exams)
                .SelectMany(s => s.Exams!) 
                .ToListAsync();

            if (exams == null || exams.Count == 0)
                return ServiceResult<IEnumerable<GetStudentExamsDto>>.Fail("Couldn't find any exams");

            var examsDto = _mapper.Map<IEnumerable<GetStudentExamsDto>>(exams);

            return ServiceResult<IEnumerable<GetStudentExamsDto>>.Ok(examsDto, "All exams retrieved successfully");
        }
    }
}