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
        private string GetStatus(DateTimeOffset start, DateTimeOffset end)
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
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.Question)
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

            // Use ExamQuestions to get questions instead of Questions property
            foreach (var dto in submission.Answers)
            {
                // Get the question from ExamQuestions
                var examQuestion = exam.ExamQuestions.FirstOrDefault(eq => eq.Question?.Id == dto.QuestionId);
                if (examQuestion?.Question == null) continue;

                var question = examQuestion.Question;

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
                            // Direct string comparison since both are strings
                            if (correctChoice.Id == dto.ChoiceId)
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
                            // No choice selected, mark stays 0
                            studentAnswer.Mark = 0;
                        }
                        break;

                    case QuestionTypes.Connection:
                        if (!string.IsNullOrEmpty(dto.ConnectionLeftId) && !string.IsNullOrEmpty(dto.ConnectionRightId))
                        {
                            var leftId = dto.ConnectionLeftId;
                            var rightId = dto.ConnectionRightId;

                            // Connection data is now directly mapped to entity
                            studentAnswer.ConnectionLeftId = leftId;
                            studentAnswer.ConnectionRightId = rightId;

                            // Your existing grading logic
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
                            // Clear connection data if incomplete
                            studentAnswer.ConnectionLeftId = null;
                            studentAnswer.ConnectionRightId = null;
                            studentAnswer.Mark = 0;
                        }
                        break;

                    case QuestionTypes.TrueOrFalse:
                        // FIXED: Handle null values properly
                        if (dto.TrueAndFalseAnswer.HasValue &&
                            question.TrueAndFalses.HasValue &&
                            question.TrueAndFalses.Value == dto.TrueAndFalseAnswer.Value)
                        {
                            studentAnswer.Mark = Math.Round((decimal)question.Degree, 2);
                        }
                        else
                        {
                            studentAnswer.Mark = 0;
                        }
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

            // Find the teacher
            var teacher = exam.Subject.TeacherSubjects?
                .FirstOrDefault(t => t.TeacherId == submission.TeacherId)?
                .Teacher;
            Guid teacherGuid;
            if (!Guid.TryParse(submission.TeacherId, out teacherGuid))
            {
                teacherGuid = Guid.Empty;
            }

            if (teacher == null)
                return ServiceResult<List<StudentExamAnswerDto>>.Fail("Teacher not found.");

            if (existingResult != null)
            {
                existingResult.TotalMark = Math.Round(totalMark, 2);
                existingResult.Percentage = percentage;
                existingResult.Status = status;
                existingResult.Date = DateOnly.FromDateTime(DateTime.Now);

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
                    Date = DateOnly.FromDateTime(DateTime.Now)
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
            try
            {
                var exams = await _ExamRepository.AsQueryable()
                                .Where(e => e.ClassId == classId)
                                .Include(e => e.Subject)
                                .Include(e => e.Teacher)
                                .ToListAsync();

                // Return empty array instead of error
                if (!exams.Any())
                {
                    return ServiceResult<IEnumerable<GetStudentExamsDto>>
                        .Ok(Enumerable.Empty<GetStudentExamsDto>(), "No exams found for this class");
                }

                // Update status logic
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

                return ServiceResult<IEnumerable<GetStudentExamsDto>>
                    .Ok(examsDto, "Exams retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<GetStudentExamsDto>>
                    .Fail("An error occurred while retrieving exams");
            }
        }

        public async Task<ServiceResult<ExamQuestionsDto>> GetExamQuestions(string examId)
        {
            var exam = await _ExamRepository.AsQueryable()
                .Include(e => e.Subject)
                .Include(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question)
                    .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
                return ServiceResult<ExamQuestionsDto>.Fail("Exam not found");

            var examDto = _mapper.Map<ExamQuestionsDto>(exam);

            return ServiceResult<ExamQuestionsDto>.Ok(examDto, "Exam questions retrieved successfully");
        }
        public async Task<ServiceResult<IEnumerable<StudentAnswerWithQuestionDto>>> GetStudentAnswersWithQuestions(string examId, string studentId)
        {
            try
            {
                var examResult = await GetExamQuestions(examId);
                if (!examResult.Success)
                    return ServiceResult<IEnumerable<StudentAnswerWithQuestionDto>>.Fail(examResult.Message);

                var studentAnswers = await _studentExamAnswerRepository.AsQueryable().Where(s => s.StudentId == studentId && s.ExamId == examId).ToListAsync();
                if (studentAnswers == null)
                    return ServiceResult<IEnumerable<StudentAnswerWithQuestionDto>>.Fail("from correction this student doesnt have any answers");

                var exam = examResult.Data;

                var result = new List<StudentAnswerWithQuestionDto>();

                foreach (var question in exam!.Questions)
                {
                    var studentAnswer = studentAnswers.FirstOrDefault(a => a.QuestionId == question.Id);

                    var studentConnections = GetStudentConnections(studentAnswer);
                    var correctConnections = GetCorrectConnections(question);

                    var dto = new StudentAnswerWithQuestionDto
                    {
                        ExamId = examId,
                        StudentId = studentId,
                        QuestionId = question.Id,
                        QuestionContent = question.Content,
                        QuestionType = question.Type,
                        QuestionDegree = question.Degree,
                        StudentMark = studentAnswer?.Mark,

                        // إجابة الطالب
                        StudentChoiceText = GetChoiceText(question.Choices!, studentAnswer?.ChoiceId!),
                        StudentTrueFalseAnswer = studentAnswer?.TrueAndFalseAnswer,
                        StudentTextAnswer = studentAnswer?.CorrectTextAnswer,
                        StudentConnectionTexts = GetConnectionTexts(studentConnections, question.Choices),

                        // الحل الصحيح
                        CorrectChoiceText = GetCorrectChoiceText(question),
                        CorrectTrueFalseAnswer = question.TrueAndFalses,
                        CorrectTextAnswer = GetCorrectTextAnswer(question),
                        CorrectConnectionTexts = GetConnectionTexts(correctConnections, question.Choices)
                    };

                    dto.IsCorrect = IsAnswerCorrect(studentAnswer!, question);
                    result.Add(dto);
                }

                return ServiceResult<IEnumerable<StudentAnswerWithQuestionDto>>.Ok(result, "Student answers with questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<StudentAnswerWithQuestionDto>>.Fail($"Error: {ex.Message}");
            }
        }

        private string? GetCorrectTextAnswer(QuestionDto question)
        {
            return question.CorrectTextAnswer;
        }

        private string? GetCorrectChoiceId(QuestionDto question)
        {
            if (question.Choices == null) return null;
            return question.Choices.FirstOrDefault(c => c.IsCorrect)?.Id;
        }

        private string? GetCorrectChoiceText(QuestionDto question)
        {
            if (question.Choices == null) return null;
            return question.Choices.FirstOrDefault(c => c.IsCorrect)?.Text;
        }

        private string? GetChoiceText(List<ChoicesDto> choices, string choiceId)
        {
            if (choices == null || string.IsNullOrEmpty(choiceId)) return null;
            return choices.FirstOrDefault(c => c.Id == choiceId)?.Text;
        }

        private List<ConnectionDto>? GetStudentConnections(StudentExamAnswer? studentAnswer)
        {
            if (studentAnswer == null || string.IsNullOrEmpty(studentAnswer.ConnectionLeftId) || string.IsNullOrEmpty(studentAnswer.ConnectionRightId))
                return null;

            return new List<ConnectionDto>
            {
                new ConnectionDto
                {
                    LeftId = studentAnswer.ConnectionLeftId,
                    RightId = studentAnswer.ConnectionRightId
                }
            };
        }

        private string? GetConnectionTexts(List<ConnectionDto>? studentConnections, List<ChoicesDto>? allChoices)
        {
            if (studentConnections == null || allChoices == null)
                return null;

            var leftChoiceText = "";
            var rightChoice= "";

            foreach (var connection in studentConnections)
            {
                leftChoiceText = allChoices.FirstOrDefault(c => c.Id == connection.LeftId)!.Text;
                rightChoice = allChoices.FirstOrDefault(c => c.Id == connection.RightId)!.Text;
            }

            return $"{leftChoiceText} , {rightChoice}";
        }

        private bool IsConnectionCorrect(StudentExamAnswer studentExamAnswer, List<ChoicesDto>? correctConnections)
        {
            if (studentExamAnswer == null || correctConnections == null)
                return false;

            var correctConns = correctConnections.ToList();

            var counter = 0;

            foreach (var correct in correctConns)
            {
                if (correct.IsCorrect) {

                    if (correct.Id == studentExamAnswer.ConnectionLeftId || correct.Id == studentExamAnswer.ConnectionRightId)
                    {
                        counter++;
                    }
                }
            }
            if (counter < 2) return false;

            return true;
        }

        private List<ConnectionDto>? GetCorrectConnections(QuestionDto question)
        {
            if (question.Choices == null || !question.Choices.Any())
                return null;

            var correctChoices = question.Choices.Where(c => c.IsCorrect).ToList();

            var connections = new List<ConnectionDto>();

            for (int i = 0; i < correctChoices.Count; i += 2)
            {
                if (i + 1 < correctChoices.Count)
                {
                    connections.Add(new ConnectionDto
                    {
                        LeftId = correctChoices[i].Id,
                        RightId = correctChoices[i + 1].Id
                    });
                }
            }

            return connections.Any() ? connections : null;
        }

        private bool IsAnswerCorrect(StudentExamAnswer studentAnswer, QuestionDto question)
        {
            switch (question.Type)
            {
                case "TrueOrFalse":
                    return studentAnswer.TrueAndFalseAnswer == question.TrueAndFalses;

                case "Choices":
                    return GetCorrectChoiceId(question) == studentAnswer.ChoiceId;

                case "Complete":
                    return !string.IsNullOrEmpty(studentAnswer.CorrectTextAnswer) &&
                           studentAnswer.CorrectTextAnswer.Trim().ToLower() ==
                           question.CorrectTextAnswer?.Trim().ToLower();

                case "Connection":
                    return IsConnectionCorrect(studentAnswer, question.Choices);

                default:
                    return false;
            }
        }

    }
}
