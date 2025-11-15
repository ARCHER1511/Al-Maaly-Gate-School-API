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

        public async Task<ServiceResult<List<StudentExamAnswerDto>>> SubmitExamAsync(StudentExamSubmissionDto submission)
        {
            var exam = await _ExamRepository.AsQueryable()
                .Include(e => e.Subject)
                 .ThenInclude(s => s.TeacherSubjects)!
                  .ThenInclude(ts => ts.Teacher)
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
                studentAnswer.CorrectTextAnswer = dto.CorrectTextAnswer;
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

                    case QuestionTypes.Connection:
                        if (dto.ConnectionId != null)
                        {
                            var leftId = dto.ConnectionId.LeftId;
                            var rightId = dto.ConnectionId.RightId;

                            // Assuming your question.Choices have IsCorrect and Ids for left-right mapping
                            var correctLeft = question.Choices?.FirstOrDefault(c => c.IsCorrect && c.Id == leftId);
                            var correctRight = question.Choices?.FirstOrDefault(c => c.IsCorrect && c.Id == rightId);

                            if (correctLeft != null && correctRight != null)
                            {
                                studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                            }
                            else
                            {
                                studentAnswer.Mark = 0;
                            }
                        }
                        else
                        {
                            studentAnswer.Mark = 0;
                        }
                        break;


                    case QuestionTypes.TrueOrFalse:
                        if (question.TrueAndFalses == dto.TrueAndFalseAnswer)
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        break;

                    case QuestionTypes.Complete:
                        {
                            if (!string.IsNullOrWhiteSpace(dto.CorrectTextAnswer) &&
                                !string.IsNullOrWhiteSpace(question.CorrectTextAnswer))
                            {
                                // Normalize (ignore case and spaces)
                                var correct = question.CorrectTextAnswer.Trim().ToLower();
                                var studentText = dto.CorrectTextAnswer.Trim().ToLower();

                                if (studentText == correct)
                                    studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                                else
                                    studentAnswer.Mark = 0;
                            }
                            else
                            {
                                studentAnswer.Mark = 0;
                            }
                            break;
                        }

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

            //if (existingResult != null)
            //{
            //    existingResult.TotalMark = Math.Round(totalMark, 2);
            //    existingResult.MinMark = exam.MinMark;
            //    existingResult.Percentage = percentage;
            //    existingResult.Status = status;
            //    existingResult.Date = DateOnly.FromDateTime(DateTime.UtcNow);
            //    existingResult.TeacherName = teacher?.FullName!;
            //    existingResult.SubjectName = exam.Subject.SubjectName;
            //    existingResult.ExamName = exam.ExamName;
            //    existingResult.StudentId = submission.StudentId;
            //    existingResult.ExamId = submission.ExamId;
            //    existingResult.SubjectName = student.FullName;

            //    _studentExamResultRepository.Update(existingResult);
            //}

            Guid teacherGuid;
            if (!Guid.TryParse(submission.TeacherId, out teacherGuid))
            {
                teacherGuid = Guid.Empty; 
            }

            var teacher = exam.Subject.TeacherSubjects?.FirstOrDefault(t => t.TeacherId == submission.TeacherId)?.Teacher;
            if (teacher == null)
                return ServiceResult<List<StudentExamAnswerDto>>.Fail("teacher not found.");

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
                    TeacherName = teacher?.FullName ?? "Teacher Name",
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