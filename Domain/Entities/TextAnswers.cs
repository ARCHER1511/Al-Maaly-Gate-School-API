namespace Domain.Entities
{
    public class TextAnswers : BaseEntity
    {
        public string? Content { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;
    }
}
