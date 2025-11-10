

namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class StudentExamSubmissionDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string ExamId { get; set; } = string.Empty;
        public List<StudentExamAnswerDto> Answers { get; set; } = new();
    }

}
