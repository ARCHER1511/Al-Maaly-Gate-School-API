namespace Application.DTOs.StudentExamAnswerDTOs
{
    public class QuestionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool? TrueAndFalses { get; set; }
        public decimal Degree { get; set; }
        public List<ChoicesDto>? Choices { get; set; } = new();
    }
}
