
namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class ExamQuestionsDto
    {
        public string ExamId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
