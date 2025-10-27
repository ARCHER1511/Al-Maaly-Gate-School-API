

namespace Domain.Entities
{
    public class Choices : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        public string? QuestionId { get; set; } = string.Empty;
        public Question Question { get; set; } = null!;
    }
}
