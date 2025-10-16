namespace Application.DTOs.QuestionDTOs
{
    public class QuestionViewDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public decimal Degree { get; set; }
        public bool IsRequired { get; set; }
        public string TeacherId { get; set; } = string.Empty;
    }
}
