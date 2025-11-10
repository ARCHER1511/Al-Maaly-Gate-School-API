using Application.DTOs.StudentExamAnswerDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Application.Services
{
    public class StudentExamAnswerService : IStudentExamAnswerService
    {
        private readonly IStudentExamAnswerRepository _studentExamAnswerRepository;
        private readonly IStudentExamResultRepository _studentExamResultRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IExamRepository _ExamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentExamAnswerService(IStudentExamAnswerRepository studentExamAnswerRepository, IStudentRepository studentRepository, IStudentExamResultRepository studentExamResultRepository, ISubjectRepository subjectRepository, IExamRepository examRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _studentExamAnswerRepository = studentExamAnswerRepository;
            _studentExamResultRepository = studentExamResultRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ExamRepository = examRepository;
            _subjectRepository = subjectRepository;
            _studentRepository = studentRepository;
        }
        private string GetStatus(DateTime start, DateTime end)
        {
            var now = DateTime.Now;

            if (now < start)
                return "Upcoming";
            else if (now >= start && now <= end)
                return "Running";
            else
                return "Finished";
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

            var exam = await _ExamRepository.AsQueryable()
                     .Include(e => e.Subject)
                     .ThenInclude(s => s.TeacherSubjects)
                     .ThenInclude(t => t.Teacher)
                     .FirstOrDefaultAsync(e => e.Id == dto.ExamId);

            if (exam == null)
                return ServiceResult<StudentExamAnswerDto>.Fail("Exam not found.");

            var student = await _studentRepository.FirstOrDefaultAsync(s => s.Id == dto.StudentId);
            if (student == null)
                return ServiceResult<StudentExamAnswerDto>.Fail("Student not found.");

            var allAnswers = await _studentExamAnswerRepository.AsQueryable()
                .Where(a => a.StudentId == dto.StudentId && a.ExamId == dto.ExamId)
                .ToListAsync();

            decimal totalMark = allAnswers.Sum(a => a.Mark ?? 0);
            decimal percentage = exam!.FullMark == 0 ? 0 : Math.Round((totalMark / (decimal)exam.FullMark) * 100, 2);
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
                existingResult.StudentName = student.FullName;
                existingResult.SubjectName = exam.Subject.SubjectName;
                existingResult.TeacherName = exam.Subject.TeacherSubjects?.FirstOrDefault()?.Teacher?.FullName ?? "[Unknown]";
                existingResult.ExamName = exam.ExamName;
                existingResult.Date = DateOnly.FromDateTime(exam.Start);
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
                    Status = status,
                    StudentName = student.FullName,
                    SubjectName = exam.Subject.SubjectName,
                    TeacherName = exam.Subject.TeacherSubjects?.FirstOrDefault()?.Teacher?.FullName ?? "[Unknown]",
                    ExamName = exam.ExamName,
                    Date = DateOnly.FromDateTime(exam.Start)
                };

                await _studentExamResultRepository.AddAsync(result);
            }

            await _unitOfWork.SaveChangesAsync();

            var viewDto = _mapper.Map<StudentExamAnswerDto>(studentAnswer);
            return ServiceResult<StudentExamAnswerDto>.Ok(viewDto, "Student answer saved and result updated successfully.");
        }

        public async Task<ServiceResult<List<StudentExamAnswerDto>>> SubmitExamAsync(StudentExamSubmissionDto submission)
        {
            var exam = await _ExamRepository.AsQueryable()
                .Include(e => e.Subject)
                .ThenInclude(s => s.Teacher)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(e => e.Id == submission.ExamId);

            if (exam == null)
                return ServiceResult<List<StudentExamAnswerDto>>.Fail("Exam not found.");

            var student = await _studentRepository.FirstOrDefaultAsync(s => s.Id == submission.StudentId);
            if (student == null)
                return ServiceResult<List<StudentExamAnswerDto>>.Fail("Student not found.");

            var existingAnswers = await _studentExamAnswerRepository.AsQueryable()
                .Where(a => a.StudentId == submission.StudentId && a.ExamId == submission.ExamId)
                .ToListAsync();

            var savedAnswers = new List<StudentExamAnswer>();

            foreach (var dto in submission.Answers)
            {
                var question = exam.Questions.FirstOrDefault(q => q.Id == dto.QuestionId);
                if (question == null) continue;

                var studentAnswer = existingAnswers.FirstOrDefault(a => a.QuestionId == dto.QuestionId)
                                    ?? new StudentExamAnswer
                                    {
                                        StudentId = submission.StudentId,
                                        ExamId = submission.ExamId,
                                        QuestionId = dto.QuestionId
                                    };

                studentAnswer.Mark = 0;
                studentAnswer.TextAnswer = dto.TextAnswer;
                studentAnswer.ChoiceId = dto.ChoiceId;
                studentAnswer.TrueAndFalseAnswer = dto.TrueAndFalseAnswer;

                switch (question.Type)
                {
                    case QuestionTypes.Choices:
                        var correctChoice = question.Choices?.FirstOrDefault(c => c.IsCorrect);

                        if (correctChoice != null && !string.IsNullOrEmpty(dto.ChoiceId))
                        {
                            if (Guid.TryParse(correctChoice.Id, out var correctChoiceGuid) &&
                                Guid.TryParse(dto.ChoiceId, out var choiceGuid))
                            {
                                studentAnswer.Mark = (correctChoiceGuid == choiceGuid)
                                    ? Math.Round((decimal)question.Degree, 2)
                                    : 0;
                            }
                        }
                        else
                        {
                            // No choice selected, mark stays 0
                            studentAnswer.Mark = 0;
                        }
                        break;

                    case QuestionTypes.TrueOrFalse:
                        if (question.TrueAndFalses == dto.TrueAndFalseAnswer)
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        break;

                    case QuestionTypes.Text:
                        studentAnswer.Mark = 0;
                        break;
                }

                if (existingAnswers.Any(a => a.QuestionId == dto.QuestionId))
                    _studentExamAnswerRepository.Update(studentAnswer);
                else
                    await _studentExamAnswerRepository.AddAsync(studentAnswer);

                savedAnswers.Add(studentAnswer);
            }

            var totalMark = savedAnswers.Sum(a => (decimal)(a.Mark ?? 0));
            var percentage = exam.FullMark > 0
                ? Math.Round((totalMark / (decimal)exam.FullMark) * 100, 2)
                : 0;
            var status = totalMark >= exam.MinMark ? "Success" : "Fail";

            var existingResult = await _studentExamResultRepository.AsQueryable()
                .FirstOrDefaultAsync(r => r.StudentId == submission.StudentId && r.ExamId == submission.ExamId);

            //if (existingResult != null && existingResult.TotalMark > 0)
            //{
            //    return ServiceResult<List<StudentExamAnswerDto>>.Fail("You have already submitted this exam.");
            //}

            if (existingResult != null)
            {
                existingResult.TotalMark = Math.Round(totalMark, 2);
                existingResult.Percentage = percentage;
                existingResult.Status = status;
                existingResult.Date = DateOnly.FromDateTime(DateTime.UtcNow);

                _studentExamResultRepository.Update(existingResult);
            }
            else
            {
                var result = new StudentExamResult
                {
                    StudentId = submission.StudentId,
                    ExamId = submission.ExamId,
                    TotalMark = Math.Round(totalMark, 2),
                    FullMark = exam.FullMark,
                    MinMark = exam.MinMark,
                    Percentage = percentage,
                    Status = status,
                    StudentName = student.FullName,
                    SubjectName = exam.Subject.SubjectName,
                    TeacherName = exam.Subject.Teacher?.FullName!,
                    ExamName = exam.ExamName,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                };

                await _studentExamResultRepository.AddAsync(result);
            }

            await _unitOfWork.SaveChangesAsync();

            var dtoList = _mapper.Map<List<StudentExamAnswerDto>>(savedAnswers);

            return ServiceResult<List<StudentExamAnswerDto>>.Ok(dtoList, "Exam submitted successfully and result updated.");
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

        public async Task<ServiceResult<IEnumerable<GetStudentExamsDto>>> GetExamsForStudentByClassId(string classId)
        {
            var exams = await _ExamRepository.AsQueryable()
                            .Where(e => e.ClassId == classId)
                            .Include(e => e.Subject)
                            .Include(e => e.Teacher)
                            .ToListAsync();

            if (!exams.Any())
                return ServiceResult<IEnumerable<GetStudentExamsDto>>.Fail("Couldn't find any exams");

            bool isChanged = false;
            foreach (var exam in exams)
            {
                var newStatus = GetStatus(exam.Start, exam.End);
                if (exam.Status != newStatus)
                {
                    exam.Status = newStatus;
                    _ExamRepository.Update(exam);
                    isChanged = true;
                }
            }
            if (isChanged)
                await _unitOfWork.SaveChangesAsync();

            var examsDto = _mapper.Map<IEnumerable<GetStudentExamsDto>>(exams);

            return ServiceResult<IEnumerable<GetStudentExamsDto>>.Ok(examsDto, "All exams retrieved successfully");

        }

        public async Task<ServiceResult<ExamQuestionsDto>> GetExamQuestions(string examId)
        {
            var exam = await _ExamRepository.AsQueryable()
                .Include(e => e.Subject)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
                return ServiceResult<ExamQuestionsDto>.Fail("Exam not found");

            var examDto = _mapper.Map<ExamQuestionsDto>(exam);

            return ServiceResult<ExamQuestionsDto>.Ok(examDto, "Exam questions retrieved successfully");
        }
    }
}