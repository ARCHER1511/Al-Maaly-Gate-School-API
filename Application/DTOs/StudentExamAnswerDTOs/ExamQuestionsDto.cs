
namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class ExamQuestionsDto
    {
        public string ExamId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
